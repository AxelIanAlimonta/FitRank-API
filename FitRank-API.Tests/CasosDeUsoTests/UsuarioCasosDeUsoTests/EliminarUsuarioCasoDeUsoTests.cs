using FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.UsuarioCasosDeUsoTests
{
    public class EliminarUsuarioCasoDeUsoTests
    {
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepo;
        private readonly EliminarUsuarioCasoDeUso _casoDeUso;

        public EliminarUsuarioCasoDeUsoTests()
        {
            _mockUsuarioRepo = new Mock<IUsuarioRepositorio>();
            _casoDeUso = new EliminarUsuarioCasoDeUso(_mockUsuarioRepo.Object);
        }

        [Fact]
        public async Task EjecutarAsync_DebeEliminarUsuarioCorrectamente()
        {
            // Arrange
            var usuarioId = 1L;
            var usuario = new Usuario
            {
                Id = usuarioId,
                Email = "usuario@test.com",
                Rol = "User",
                Estado = "Activo"
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(usuarioId)).ReturnsAsync(usuario);
            _mockUsuarioRepo.Setup(r => r.EliminarAsync(usuario)).Returns(Task.CompletedTask);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(usuarioId);

            // Assert
            resultado.Should().BeTrue();
            _mockUsuarioRepo.Verify(r => r.ObtenerPorIdAsync(usuarioId), Times.Once);
            _mockUsuarioRepo.Verify(r => r.EliminarAsync(usuario), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_DebeRetornarFalseSiUsuarioNoExiste()
        {
            // Arrange
            var usuarioId = 999L;

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(usuarioId)).ReturnsAsync((Usuario?)null);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(usuarioId);

            // Assert
            resultado.Should().BeFalse();
            _mockUsuarioRepo.Verify(r => r.ObtenerPorIdAsync(usuarioId), Times.Once);
            _mockUsuarioRepo.Verify(r => r.EliminarAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_DebeEliminarUsuarioPorId()
        {
            // Arrange
            var usuarioId = 5L;
            var usuario = new Usuario
            {
                Id = usuarioId,
                Email = "usuario5@test.com"
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(usuarioId)).ReturnsAsync(usuario);
            _mockUsuarioRepo.Setup(r => r.EliminarAsync(usuario)).Returns(Task.CompletedTask);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(usuarioId);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public async Task EjecutarAsync_DebeLlamarEliminarConUsuarioCorrecto()
        {
            // Arrange
            var usuarioId = 10L;
            var usuario = new Usuario
            {
                Id = usuarioId,
                Email = "test@test.com",
                Rol = "Admin"
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(usuarioId)).ReturnsAsync(usuario);
            _mockUsuarioRepo.Setup(r => r.EliminarAsync(usuario)).Returns(Task.CompletedTask);

            // Act
            await _casoDeUso.EjecutarAsync(usuarioId);

            // Assert
            _mockUsuarioRepo.Verify(r => r.EliminarAsync(It.Is<Usuario>(u =>
                u.Id == usuarioId &&
                u.Email == "test@test.com"
            )), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_DebeRetornarTrueSoloSiSeEliminaCorrectamente()
        {
            // Arrange
            var usuarioId = 1L;
            var usuario = new Usuario { Id = usuarioId };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(usuarioId)).ReturnsAsync(usuario);
            _mockUsuarioRepo.Setup(r => r.EliminarAsync(usuario)).Returns(Task.CompletedTask);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(usuarioId);

            // Assert
            resultado.Should().BeTrue();
        }
    }
}
