using FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso;
using FitRank_API.Application.DTOs.ProfesorDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.ProfesorCasosDeUsoTests
{
    public class ObtenerEstadisticasProfesoresCasoDeUsoTests
    {
        private readonly Mock<ISolicitudRutinaProfesorRepositorio> _mockRepo;
        private readonly ObtenerEstadisticasProfesoresCasoDeUso _casoDeUso;

        public ObtenerEstadisticasProfesoresCasoDeUsoTests()
        {
            _mockRepo = new Mock<ISolicitudRutinaProfesorRepositorio>();
            _casoDeUso = new ObtenerEstadisticasProfesoresCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarEstadisticasCompletas()
        {
            // Arrange
            var profesorSolicitado = new Profesor
            {
                Id = 1,
                Nombre = "Juan",
                Apellido = "Pérez",
                Solicitudes = new List<SolicitudRutinaProfesor> { new(), new(), new() }
            };

            var profesorPendientes = new Profesor
            {
                Id = 2,
                Nombre = "María",
                Apellido = "González",
                Solicitudes = new List<SolicitudRutinaProfesor>
                {
                    new() { Estado = EstadoSolicitud.Pendiente },
                    new() { Estado = EstadoSolicitud.Pendiente }
                }
            };

            var profesorCumplidor = new Profesor
            {
                Id = 3,
                Nombre = "Carlos",
                Apellido = "López",
                Solicitudes = new List<SolicitudRutinaProfesor>
                {
                    new() { Estado = EstadoSolicitud.TomadaPorProfesor },
                    new() { Estado = EstadoSolicitud.Rechazada }
                }
            };

            var profesorValorado = new Profesor
            {
                Id = 4,
                Nombre = "Ana",
                Apellido = "Martínez"
            };

            _mockRepo.Setup(r => r.ObtenerProfesorMasSolicitadoAsync()).ReturnsAsync(profesorSolicitado);
            _mockRepo.Setup(r => r.ObtenerProfesorConMasPendientesAsync()).ReturnsAsync(profesorPendientes);
            _mockRepo.Setup(r => r.ObtenerProfesorMasCumplidorAsync()).ReturnsAsync(profesorCumplidor);
            _mockRepo.Setup(r => r.ObtenerProfesorMejorPromedioValoracionesAsync())
                .ReturnsAsync((profesorValorado, 4.8));

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.TopSolicitado.Should().NotBeNull();
            resultado.TopSolicitado!.NombreProfesor.Should().Be("Juan Pérez");
            resultado.TopSolicitado.CantidadSolicitudes.Should().Be(3);

            resultado.TopPendientes.Should().NotBeNull();
            resultado.TopPendientes!.NombreProfesor.Should().Be("María González");
            resultado.TopPendientes.Pendientes.Should().Be(2);

            resultado.TopCumplidor.Should().NotBeNull();
            resultado.TopCumplidor!.NombreProfesor.Should().Be("Carlos López");
            resultado.TopCumplidor.Completadas.Should().Be(2);

            resultado.TopValorado.Should().NotBeNull();
            resultado.TopValorado!.NombreProfesor.Should().Be("Ana Martínez");
            resultado.TopValorado.PromedioValoracion.Should().Be(4.8);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarNullsCuandoNoHayDatos()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObtenerProfesorMasSolicitadoAsync()).ReturnsAsync((Profesor?)null);
            _mockRepo.Setup(r => r.ObtenerProfesorConMasPendientesAsync()).ReturnsAsync((Profesor?)null);
            _mockRepo.Setup(r => r.ObtenerProfesorMasCumplidorAsync()).ReturnsAsync((Profesor?)null);
            _mockRepo.Setup(r => r.ObtenerProfesorMejorPromedioValoracionesAsync())
                .ReturnsAsync(((Profesor?)null, (double?)null));

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.TopSolicitado.Should().BeNull();
            resultado.TopPendientes.Should().BeNull();
            resultado.TopCumplidor.Should().BeNull();
            resultado.TopValorado.Should().BeNull();
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarTodosLosMetodosDelRepositorio()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObtenerProfesorMasSolicitadoAsync()).ReturnsAsync((Profesor?)null);
            _mockRepo.Setup(r => r.ObtenerProfesorConMasPendientesAsync()).ReturnsAsync((Profesor?)null);
            _mockRepo.Setup(r => r.ObtenerProfesorMasCumplidorAsync()).ReturnsAsync((Profesor?)null);
            _mockRepo.Setup(r => r.ObtenerProfesorMejorPromedioValoracionesAsync())
                .ReturnsAsync(((Profesor?)null, (double?)null));

            // Act
            await _casoDeUso.Ejecutar();

            // Assert
            _mockRepo.Verify(r => r.ObtenerProfesorMasSolicitadoAsync(), Times.Once);
            _mockRepo.Verify(r => r.ObtenerProfesorConMasPendientesAsync(), Times.Once);
            _mockRepo.Verify(r => r.ObtenerProfesorMasCumplidorAsync(), Times.Once);
            _mockRepo.Verify(r => r.ObtenerProfesorMejorPromedioValoracionesAsync(), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeContarCorrectamenteEstadosPendientes()
        {
            // Arrange
            var profesor = new Profesor
            {
                Nombre = "Test",
                Apellido = "Profesor",
                Solicitudes = new List<SolicitudRutinaProfesor>
                {
                    new() { Estado = EstadoSolicitud.Pendiente },
                    new() { Estado = EstadoSolicitud.TomadaPorProfesor },
                    new() { Estado = EstadoSolicitud.Pendiente },
                    new() { Estado = EstadoSolicitud.Finalizada }
                }
            };

            _mockRepo.Setup(r => r.ObtenerProfesorMasSolicitadoAsync()).ReturnsAsync((Profesor?)null);
            _mockRepo.Setup(r => r.ObtenerProfesorConMasPendientesAsync()).ReturnsAsync(profesor);
            _mockRepo.Setup(r => r.ObtenerProfesorMasCumplidorAsync()).ReturnsAsync((Profesor?)null);
            _mockRepo.Setup(r => r.ObtenerProfesorMejorPromedioValoracionesAsync())
                .ReturnsAsync(((Profesor?)null, (double?)null));

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.TopPendientes.Should().NotBeNull();
            resultado.TopPendientes!.Pendientes.Should().Be(2);
        }

        [Fact]
        public async Task Ejecutar_DebeContarCorrectamenteCompletadas()
        {
            // Arrange
            var profesor = new Profesor
            {
                Nombre = "Test",
                Apellido = "Cumplidor",
                Solicitudes = new List<SolicitudRutinaProfesor>
                {
                    new() { Estado = EstadoSolicitud.TomadaPorProfesor },
                    new() { Estado = EstadoSolicitud.Rechazada },
                    new() { Estado = EstadoSolicitud.Pendiente },
                    new() { Estado = EstadoSolicitud.TomadaPorProfesor }
                }
            };

            _mockRepo.Setup(r => r.ObtenerProfesorMasSolicitadoAsync()).ReturnsAsync((Profesor?)null);
            _mockRepo.Setup(r => r.ObtenerProfesorConMasPendientesAsync()).ReturnsAsync((Profesor?)null);
            _mockRepo.Setup(r => r.ObtenerProfesorMasCumplidorAsync()).ReturnsAsync(profesor);
            _mockRepo.Setup(r => r.ObtenerProfesorMejorPromedioValoracionesAsync())
                .ReturnsAsync(((Profesor?)null, (double?)null));

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.TopCumplidor.Should().NotBeNull();
            resultado.TopCumplidor!.Completadas.Should().Be(3);
        }
    }
}
