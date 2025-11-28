using FluentAssertions;
using FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.AsistenciaCasosDeUsoTests
{
    public class ObtenerAsistenciasPorDiaCasoDeUsoTests
    {
        private readonly Mock<IAsistenciaRepositorio> _mockRepositorio;
        private readonly ObtenerAsistenciasPorDiaCasoDeUso _casoDeUso;

        public ObtenerAsistenciasPorDiaCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IAsistenciaRepositorio>();
            _casoDeUso = new ObtenerAsistenciasPorDiaCasoDeUso(_mockRepositorio.Object);
        }

        [Fact]
        public async Task DeberiaAgruparAsistenciasPorDia()
        {
            // Arrange
            var gimnasioId = 10L;
            var fecha1 = new DateTime(2024, 1, 1);
            var fecha2 = new DateTime(2024, 1, 2);

            var asistencias = new List<Asistencia>
            {
                new Asistencia { Id = 1, Fecha = fecha1, UsuarioId = 1, GimnasioId = gimnasioId },
                new Asistencia { Id = 2, Fecha = fecha1, UsuarioId = 2, GimnasioId = gimnasioId },
                new Asistencia { Id = 3, Fecha = fecha1, UsuarioId = 3, GimnasioId = gimnasioId },
                new Asistencia { Id = 4, Fecha = fecha2, UsuarioId = 1, GimnasioId = gimnasioId },
                new Asistencia { Id = 5, Fecha = fecha2, UsuarioId = 2, GimnasioId = gimnasioId }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioYRangoAsync(gimnasioId, null, null))
                .ReturnsAsync(asistencias);

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            
            var dia1 = resultado.FirstOrDefault(r => r.Fecha.Date == fecha2.Date);
            dia1.Should().NotBeNull();
            dia1.Cantidad.Should().Be(2);
            
            var dia2 = resultado.FirstOrDefault(r => r.Fecha.Date == fecha1.Date);
            dia2.Should().NotBeNull();
            dia2.Cantidad.Should().Be(3);
        }

        [Fact]
        public async Task DeberiaOrdenarPorFechaDescendente()
        {
            // Arrange
            var gimnasioId = 10L;
            var fecha1 = new DateTime(2024, 1, 1);
            var fecha2 = new DateTime(2024, 1, 2);
            var fecha3 = new DateTime(2024, 1, 3);

            var asistencias = new List<Asistencia>
            {
                new Asistencia { Id = 1, Fecha = fecha1, UsuarioId = 1, GimnasioId = gimnasioId },
                new Asistencia { Id = 2, Fecha = fecha3, UsuarioId = 2, GimnasioId = gimnasioId },
                new Asistencia { Id = 3, Fecha = fecha2, UsuarioId = 3, GimnasioId = gimnasioId }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioYRangoAsync(gimnasioId, null, null))
                .ReturnsAsync(asistencias);

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(3);
            resultado[0].Fecha.Date.Should().Be(fecha3.Date);
            resultado[1].Fecha.Date.Should().Be(fecha2.Date);
            resultado[2].Fecha.Date.Should().Be(fecha1.Date);
        }

        [Fact]
        public async Task DeberiaRetornarListaVaciaCuandoNoHayAsistencias()
        {
            // Arrange
            var gimnasioId = 10L;
            var asistenciasVacias = new List<Asistencia>();

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioYRangoAsync(gimnasioId, null, null))
                .ReturnsAsync(asistenciasVacias);

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task DeberiaFiltrarPorRangoDeFechas()
        {
            // Arrange
            var gimnasioId = 10L;
            var desde = new DateTime(2024, 1, 1);
            var hasta = new DateTime(2024, 1, 31);

            var asistencias = new List<Asistencia>
            {
                new Asistencia { Id = 1, Fecha = new DateTime(2024, 1, 15), UsuarioId = 1, GimnasioId = gimnasioId },
                new Asistencia { Id = 2, Fecha = new DateTime(2024, 1, 20), UsuarioId = 2, GimnasioId = gimnasioId }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioYRangoAsync(gimnasioId, desde, hasta))
                .ReturnsAsync(asistencias);

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId, desde, hasta);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            _mockRepositorio.Verify(r => r.ObtenerPorGimnasioYRangoAsync(gimnasioId, desde, hasta), Times.Once);
        }

        [Fact]
        public async Task DeberiaContarCorrectamenteCuandoHayVariasAsistenciasPorDia()
        {
            // Arrange
            var gimnasioId = 10L;
            var fecha = new DateTime(2024, 1, 1);

            var asistencias = new List<Asistencia>
            {
                new Asistencia { Id = 1, Fecha = fecha, UsuarioId = 1, GimnasioId = gimnasioId },
                new Asistencia { Id = 2, Fecha = fecha, UsuarioId = 2, GimnasioId = gimnasioId },
                new Asistencia { Id = 3, Fecha = fecha, UsuarioId = 3, GimnasioId = gimnasioId },
                new Asistencia { Id = 4, Fecha = fecha, UsuarioId = 4, GimnasioId = gimnasioId },
                new Asistencia { Id = 5, Fecha = fecha, UsuarioId = 5, GimnasioId = gimnasioId }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioYRangoAsync(gimnasioId, null, null))
                .ReturnsAsync(asistencias);

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(1);
            resultado[0].Cantidad.Should().Be(5);
            resultado[0].Fecha.Date.Should().Be(fecha.Date);
        }
    }
}
