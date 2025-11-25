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
    public class ActualizarPuntajeCasoDeUsoTests
    {
        private readonly Mock<IPuntajeRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly ActualizarPuntajeCasoDeUso _casoDeUso;

        public ActualizarPuntajeCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IPuntajeRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PuntajeProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ActualizarPuntajeCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaActualizarPuntajeCorrectamente()
        {
            // Arrange
            var puntajeActualizado = new ActualizarPuntajeDTO
            {
                Id = 1,
                SocioId = 1,
                Motivo = "Motivo actualizado",
                Fecha = DateTime.Now,
                Valor = 15
            };

            var puntajeEntidad = new Puntaje
            {
                Id = 1,
                SocioId = 1,
                Motivo = "Motivo actualizado",
                Fecha = puntajeActualizado.Fecha,
                Valor = 15
            };

            _mockRepositorio.Setup(r => r.ActualizarAsync(It.IsAny<Puntaje>()))
                .ReturnsAsync(puntajeEntidad);

            // Act
            var resultado = await _casoDeUso.Ejecutar(puntajeActualizado);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Id.Should().Be(1);
            resultado.Motivo.Should().Be("Motivo actualizado");
            resultado.Valor.Should().Be(15);
            _mockRepositorio.Verify(r => r.ActualizarAsync(It.IsAny<Puntaje>()), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarNullCuandoPuntajeNoExiste()
        {
            // Arrange
            var puntajeActualizado = new ActualizarPuntajeDTO
            {
                Id = 999,
                SocioId = 1,
                Motivo = "Motivo",
                Fecha = DateTime.Now,
                Valor = 10
            };

            _mockRepositorio.Setup(r => r.ActualizarAsync(It.IsAny<Puntaje>()))
                .ReturnsAsync((Puntaje?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(puntajeActualizado);

            // Assert
            resultado.Should().BeNull();
            _mockRepositorio.Verify(r => r.ActualizarAsync(It.IsAny<Puntaje>()), Times.Once);
        }

        [Fact]
        public async Task DebeMapearCorrectamenteTodosLosCampos()
        {
            // Arrange
            var fecha = new DateTime(2024, 5, 10, 14, 30, 0);
            var dto = new ActualizarPuntajeDTO
            {
                Id = 25,
                SocioId = 100,
                Motivo = "Buen rendimiento",
                Fecha = fecha,
                Valor = 20
            };

            var puntajeActualizado = new Puntaje
            {
                Id = 25,
                SocioId = 100,
                Motivo = "Buen rendimiento",
                Fecha = fecha,
                Valor = 20
            };

            _mockRepositorio.Setup(r => r.ActualizarAsync(It.IsAny<Puntaje>()))
                .ReturnsAsync(puntajeActualizado);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Id.Should().Be(25);
            resultado.SocioId.Should().Be(100);
            resultado.Motivo.Should().Be("Buen rendimiento");
            resultado.Fecha.Should().Be(fecha);
            resultado.Valor.Should().Be(20);
        }

        [Fact]
        public async Task DebeLlamarRepositorioConDatosCorrectos()
        {
            // Arrange
            var dto = new ActualizarPuntajeDTO
            {
                Id = 5,
                SocioId = 50,
                Motivo = "Test",
                Fecha = DateTime.UtcNow,
                Valor = 30
            };

            _mockRepositorio.Setup(r => r.ActualizarAsync(It.IsAny<Puntaje>()))
                .ReturnsAsync((Puntaje p) => p);

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            _mockRepositorio.Verify(r => r.ActualizarAsync(
                It.Is<Puntaje>(p => p.Id == dto.Id && 
                                   p.SocioId == dto.SocioId && 
                                   p.Motivo == dto.Motivo && 
                                   p.Valor == dto.Valor)), 
                Times.Once);
        }

        [Fact]
        public async Task DeberiaActualizarPuntajesConDiferentesValores()
        {
            // Arrange
            var dto1 = new ActualizarPuntajeDTO { Id = 1, SocioId = 1, Motivo = "M1", Fecha = DateTime.Now, Valor = 5 };
            var dto2 = new ActualizarPuntajeDTO { Id = 2, SocioId = 2, Motivo = "M2", Fecha = DateTime.Now, Valor = 50 };

            _mockRepositorio.Setup(r => r.ActualizarAsync(It.Is<Puntaje>(p => p.Id == 1)))
                .ReturnsAsync(new Puntaje { Id = 1, SocioId = 1, Motivo = "M1", Fecha = dto1.Fecha, Valor = 5 });

            _mockRepositorio.Setup(r => r.ActualizarAsync(It.Is<Puntaje>(p => p.Id == 2)))
                .ReturnsAsync(new Puntaje { Id = 2, SocioId = 2, Motivo = "M2", Fecha = dto2.Fecha, Valor = 50 });

            // Act
            var resultado1 = await _casoDeUso.Ejecutar(dto1);
            var resultado2 = await _casoDeUso.Ejecutar(dto2);

            // Assert
            resultado1!.Valor.Should().Be(5);
            resultado2!.Valor.Should().Be(50);
        }

        [Fact]
        public async Task DeberiaRetornarTipoObtenerPuntajeDTO()
        {
            // Arrange
            var dto = new ActualizarPuntajeDTO
            {
                Id = 1,
                SocioId = 1,
                Motivo = "Test",
                Fecha = DateTime.Now,
                Valor = 10
            };

            _mockRepositorio.Setup(r => r.ActualizarAsync(It.IsAny<Puntaje>()))
                .ReturnsAsync((Puntaje p) => p);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeAssignableTo<FitRank_API.Application.DTOs.PuntajeDTOs.ObtenerPuntajeDTO>();
        }
    }
}
