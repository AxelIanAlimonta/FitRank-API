using FluentAssertions;
using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Infrastructure.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.InvitacionCasosDeUsoTests
{
    public class EliminarInvitacionCasoDeUsoTests
    {
        private readonly Mock<IInvitacionRepositorio> _mockRepositorio;
        private readonly EliminarInvitacionCasoDeUso _casoDeUso;

        public EliminarInvitacionCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IInvitacionRepositorio>();
            _casoDeUso = new EliminarInvitacionCasoDeUso(_mockRepositorio.Object);
        }

        [Fact]
        public async Task DeberiaEliminarInvitacionExitosamente()
        {
            // Arrange
            var invitacionId = 1L;
            _mockRepositorio.Setup(r => r.Eliminar(invitacionId))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Ejecutar(invitacionId);

            // Assert
            resultado.Should().BeTrue();
            _mockRepositorio.Verify(r => r.Eliminar(invitacionId), Times.Once);
        }

        [Fact]
        public async Task DeberiaLanzarExcepcionCuandoInvitacionNoExiste()
        {
            // Arrange
            var invitacionId = 999L;
            _mockRepositorio.Setup(r => r.Eliminar(invitacionId))
                .ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _casoDeUso.Ejecutar(invitacionId);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("No se encontró la invitación para eliminar.");
          
        }
    }
}
