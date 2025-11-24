using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.PuntajeCasosDeUso;
using FitRank_API.Application.DTOs.PuntajeDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.PuntajeCasosDeUsoTests
{
    public class AgregarPuntajeCasoDeUsoTests
    {
        private readonly Mock<IPuntajeRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly AgregarPuntajeCasoDeUso _casoDeUso;

        public AgregarPuntajeCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IPuntajeRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PuntajeProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new AgregarPuntajeCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaAgregarPuntajeCorrectamente()
        {
            // Arrange
            var nuevoPuntaje = new AgregarPuntajeDTO
            {
                SocioId = 1,
                Motivo = "Asistencia perfecta",
                Fecha = DateTime.Now,
                Valor = 10
            };

            var puntajeEntidad = new Puntaje
            {
                Id = 1,
                SocioId = 1,
                Motivo = "Asistencia perfecta",
                Fecha = nuevoPuntaje.Fecha,
                Valor = 10
            };

            _mockRepositorio.Setup(r => r.AgregarAsync(It.IsAny<Puntaje>()))
                .ReturnsAsync(puntajeEntidad);

            // Act
            var resultado = await _casoDeUso.Ejecutar(nuevoPuntaje);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(1);
            resultado.SocioId.Should().Be(1);
            resultado.Motivo.Should().Be("Asistencia perfecta");
            resultado.Valor.Should().Be(10);
            _mockRepositorio.Verify(r => r.AgregarAsync(It.IsAny<Puntaje>()), Times.Once);
        }

        [Fact]
        public async Task DebeMapearCorrectamenteTodosLosCampos()
        {
            // Arrange
            var fecha = new DateTime(2024, 3, 15, 10, 0, 0);
            var dto = new AgregarPuntajeDTO
            {
                SocioId = 200,
                Motivo = "Completó rutina avanzada",
                Fecha = fecha,
                Valor = 25
            };

            _mockRepositorio.Setup(r => r.AgregarAsync(It.IsAny<Puntaje>()))
                .ReturnsAsync((Puntaje p) => { p.Id = 10; return p; });

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.SocioId.Should().Be(200);
            resultado.Motivo.Should().Be("Completó rutina avanzada");
            resultado.Fecha.Should().Be(fecha);
            resultado.Valor.Should().Be(25);
        }

        [Fact]
        public async Task DebeLlamarRepositorioConDatosCorrectos()
        {
            // Arrange
            var dto = new AgregarPuntajeDTO
            {
                SocioId = 75,
                Motivo = "Bonus semanal",
                Fecha = DateTime.UtcNow,
                Valor = 15
            };

            _mockRepositorio.Setup(r => r.AgregarAsync(It.IsAny<Puntaje>()))
                .ReturnsAsync((Puntaje p) => { p.Id = 1; return p; });

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            _mockRepositorio.Verify(r => r.AgregarAsync(
                It.Is<Puntaje>(p => p.SocioId == dto.SocioId && 
                                   p.Motivo == dto.Motivo && 
                                   p.Valor == dto.Valor)), 
                Times.Once);
        }

        [Fact]
        public async Task DeberiaAgregarPuntajesConDiferentesValores()
        {
            // Arrange
            var dto1 = new AgregarPuntajeDTO { SocioId = 1, Motivo = "Bajo", Fecha = DateTime.Now, Valor = 1 };
            var dto2 = new AgregarPuntajeDTO { SocioId = 2, Motivo = "Alto", Fecha = DateTime.Now, Valor = 100 };

            _mockRepositorio.Setup(r => r.AgregarAsync(It.IsAny<Puntaje>()))
                .ReturnsAsync((Puntaje p) => { p.Id = 1; return p; });

            // Act
            var resultado1 = await _casoDeUso.Ejecutar(dto1);
            var resultado2 = await _casoDeUso.Ejecutar(dto2);

            // Assert
            resultado1.Valor.Should().Be(1);
            resultado2.Valor.Should().Be(100);
        }

        [Fact]
        public async Task DeberiaManteneFechaOriginalDelDTO()
        {
            // Arrange
            var fechaEspecifica = new DateTime(2023, 12, 25, 18, 45, 0);
            var dto = new AgregarPuntajeDTO
            {
                SocioId = 10,
                Motivo = "Navidad bonus",
                Fecha = fechaEspecifica,
                Valor = 50
            };

            _mockRepositorio.Setup(r => r.AgregarAsync(It.IsAny<Puntaje>()))
                .ReturnsAsync((Puntaje p) => { p.Id = 1; return p; });

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Fecha.Should().Be(fechaEspecifica);
        }

        [Fact]
        public async Task DeberiaRetornarTipoObtenerPuntajeDTO()
        {
            // Arrange
            var dto = new AgregarPuntajeDTO
            {
                SocioId = 1,
                Motivo = "Test",
                Fecha = DateTime.Now,
                Valor = 10
            };

            _mockRepositorio.Setup(r => r.AgregarAsync(It.IsAny<Puntaje>()))
                .ReturnsAsync((Puntaje p) => { p.Id = 1; return p; });

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().BeOfType<FitRank_API.Application.DTOs.PuntajeDTOs.ObtenerPuntajeDTO>();
        }
    }
}
