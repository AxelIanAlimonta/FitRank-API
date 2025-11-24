using FitRank_API.Application.CasosDeUso.SocioCasosDeUso;
using FitRank_API.Application.DTOs.SocioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.SocioCasosDeUsoTests
{
    public class EditarPerfilSocioCasoDeUsoTests
    {
        private readonly Mock<ISocioRepositorio> _mockRepo;
        private readonly EditarPerfilSocioCasoDeUso _casoDeUso;

        public EditarPerfilSocioCasoDeUsoTests()
        {
            _mockRepo = new Mock<ISocioRepositorio>();
            _casoDeUso = new EditarPerfilSocioCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeEditarPerfilCorrectamente()
        {
            // Arrange
            var socioId = 1L;
            var dto = new EditarPerfilSocioDTO
            {
                Nombre = "Juan",
                Apellido = "Pérez",
                Sexo = "M",
                FotoUrl = "https://foto.com/juan.jpg",
                Altura = 180,
                Peso = 75
            };

            var socio = new Socio { Id = socioId, Nombre = "Viejo", Apellido = "Viejo" };
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(socioId)).ReturnsAsync(socio);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<Socio>())).ReturnsAsync((Socio s) => s);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId, dto);

            // Assert
            resultado.Should().BeTrue();
            socio.Nombre.Should().Be(dto.Nombre);
            socio.Apellido.Should().Be(dto.Apellido);
            socio.Sexo.Should().Be(dto.Sexo);
            socio.FotoDePerfil.Should().Be(dto.FotoUrl);
            socio.Altura.Should().Be(dto.Altura);
            socio.Peso.Should().Be(dto.Peso);
            _mockRepo.Verify(r => r.ActualizarAsync(socio), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarFalseSiSocioNoExiste()
        {
            // Arrange
            var socioId = 999L;
            var dto = new EditarPerfilSocioDTO { Nombre = "Test" };
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(socioId)).ReturnsAsync((Socio?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId, dto);

            // Assert
            resultado.Should().BeFalse();
            _mockRepo.Verify(r => r.ActualizarAsync(It.IsAny<Socio>()), Times.Never);
        }

        [Fact]
        public async Task Ejecutar_DebeActualizarTodosLosCamposEditables()
        {
            // Arrange
            var socioId = 1L;
            var dto = new EditarPerfilSocioDTO
            {
                Nombre = "Nuevo",
                Apellido = "Apellido",
                Sexo = "F",
                FotoUrl = "url",
                Altura = 170,
                Peso = 60
            };

            var socio = new Socio { Id = socioId };
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(socioId)).ReturnsAsync(socio);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<Socio>())).ReturnsAsync((Socio s) => s);

            // Act
            await _casoDeUso.Ejecutar(socioId, dto);

            // Assert
            socio.Nombre.Should().Be("Nuevo");
            socio.Apellido.Should().Be("Apellido");
            socio.Sexo.Should().Be("F");
            socio.FotoDePerfil.Should().Be("url");
            socio.Altura.Should().Be(170);
            socio.Peso.Should().Be(60);
        }
    }
}
