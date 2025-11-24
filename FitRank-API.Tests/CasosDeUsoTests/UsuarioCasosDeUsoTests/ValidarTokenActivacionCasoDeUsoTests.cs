using FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace FitRank_API.Tests.CasosDeUsoTests.UsuarioCasosDeUsoTests
{
    public class ValidarTokenActivacionCasoDeUsoTests
    {
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepo;
        private readonly ValidarTokenActivacionCasoDeUso _casoDeUso;

        public ValidarTokenActivacionCasoDeUsoTests()
        {
            _mockUsuarioRepo = new Mock<IUsuarioRepositorio>();
            _casoDeUso = new ValidarTokenActivacionCasoDeUso(_mockUsuarioRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarTrueSiTokenEsValido()
        {
            // Arrange
            var token = "token_valido_123";

            var usuario = new Usuario
            {
                Id = 1,
                Email = "usuario@test.com",
                TokenRecuperacion = token,
                TokenExpira = DateTime.UtcNow.AddHours(1),
                EsActivado = false
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync(usuario);

            // Act
            var resultado = await _casoDeUso.Ejecutar(token);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarFalseSiTokenNoExiste()
        {
            // Arrange
            var token = "token_inexistente";

            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync((Usuario?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(token);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarFalseSiTokenExpirado()
        {
            // Arrange
            var token = "token_expirado";

            var usuario = new Usuario
            {
                Id = 1,
                Email = "usuario@test.com",
                TokenRecuperacion = token,
                TokenExpira = DateTime.UtcNow.AddHours(-1), // Expirado
                EsActivado = false
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync((Usuario?)null); // La condición no coincide porque está expirado

            // Act
            var resultado = await _casoDeUso.Ejecutar(token);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarFalseSiUsuarioYaActivado()
        {
            // Arrange
            var token = "token_valido";

            var usuario = new Usuario
            {
                Id = 1,
                Email = "usuario@test.com",
                TokenRecuperacion = token,
                TokenExpira = DateTime.UtcNow.AddHours(1),
                EsActivado = true // Ya activado
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync((Usuario?)null); // La condición no coincide porque ya está activado

            // Act
            var resultado = await _casoDeUso.Ejecutar(token);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task Ejecutar_DebeValidarTodasLasCondiciones()
        {
            // Arrange
            var token = "token_valido";

            var usuario = new Usuario
            {
                Id = 1,
                Email = "usuario@test.com",
                TokenRecuperacion = token,
                TokenExpira = DateTime.UtcNow.AddMinutes(30),
                EsActivado = false
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync(usuario);

            // Act
            var resultado = await _casoDeUso.Ejecutar(token);

            // Assert
            resultado.Should().BeTrue();
            _mockUsuarioRepo.Verify(r => r.ObtenerPorCondicionAsync(
                It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioConCondiciones()
        {
            // Arrange
            var token = "test_token";

            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync((Usuario?)null);

            // Act
            await _casoDeUso.Ejecutar(token);

            // Assert
            _mockUsuarioRepo.Verify(r => r.ObtenerPorCondicionAsync(
                It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
        }
    }
}
