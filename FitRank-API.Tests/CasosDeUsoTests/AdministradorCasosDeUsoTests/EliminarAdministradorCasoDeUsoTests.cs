using FluentAssertions;
using FitRank_API.Application.CasosDeUso.AdministradorCasosDeUso;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.AdministradorCasosDeUsoTests
{
    public class EliminarAdministradorCasoDeUsoTests
    {
        private readonly Mock<IAdministradorRepositorio> _mockRepositorio;
        private readonly EliminarAdministradorCasoDeUso _casoDeUso;

        public EliminarAdministradorCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IAdministradorRepositorio>();
            _casoDeUso = new EliminarAdministradorCasoDeUso(_mockRepositorio.Object);
        }

        [Fact]
        public async Task DeberiaEliminarAdministradorCuandoExiste()
        {
            // Arrange
            long adminId = 1;
            var adminExistente = new Administrador
            {
                Id = 1,
                Nombre = "Admin",
                Apellido = "Test",
                Email = "admin@test.com"
            };

            _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(adminId))
                .ReturnsAsync(adminExistente);
            _mockRepositorio.Setup(r => r.EliminarAsync(It.IsAny<Administrador>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _casoDeUso.Ejecutar(adminId);

            // Assert
            resultado.Should().BeTrue();
            _mockRepositorio.Verify(r => r.ObtenerPorIdAsync(adminId), Times.Once);
            _mockRepositorio.Verify(r => r.EliminarAsync(adminExistente), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarFalseCuandoAdministradorNoExiste()
        {
            // Arrange
            long adminId = 999;
            _mockRepositorio.Setup(r => r.ObtenerPorIdAsync(adminId))
                .ReturnsAsync((Administrador?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(adminId);

            // Assert
            resultado.Should().BeFalse();
            _mockRepositorio.Verify(r => r.ObtenerPorIdAsync(adminId), Times.Once);
            _mockRepositorio.Verify(r => r.EliminarAsync(It.IsAny<Administrador>()), Times.Never);
        }
    }
}
