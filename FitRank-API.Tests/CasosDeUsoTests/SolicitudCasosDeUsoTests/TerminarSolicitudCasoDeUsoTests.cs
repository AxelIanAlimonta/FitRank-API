using FitRank_API.Application.CasosDeUso.SolicitudCasosDeUso;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.SolicitudCasosDeUsoTests
{
    public class TerminarSolicitudCasoDeUsoTests
    {
        private readonly Mock<ISolicitudRutinaProfesorRepositorio> _mockRepo;
        private readonly TerminarSolicitudCasoDeUso _casoDeUso;

        public TerminarSolicitudCasoDeUsoTests()
        {
            _mockRepo = new Mock<ISolicitudRutinaProfesorRepositorio>();
            _casoDeUso = new TerminarSolicitudCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task EjecutarAsync_DebeTerminarSolicitudCorrectamente()
        {
            // Arrange
            var solicitudId = 1L;

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                Estado = EstadoSolicitud.TomadaPorProfesor,
                FechaResolucion = null
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(solicitudId);

            // Assert
            resultado.Should().BeTrue();
            solicitud.Estado.Should().Be(EstadoSolicitud.Finalizada);
            solicitud.FechaResolucion.Should().NotBeNull();
            solicitud.FechaResolucion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            _mockRepo.Verify(r => r.ActualizarAsync(solicitud), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_DebeRetornarFalseSiSolicitudNoExiste()
        {
            // Arrange
            var solicitudId = 999L;

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync((SolicitudRutinaProfesor?)null);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(solicitudId);

            // Assert
            resultado.Should().BeFalse();
            _mockRepo.Verify(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_DebeCambiarEstadoAFinalizada()
        {
            // Arrange
            var solicitudId = 1L;

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                Estado = EstadoSolicitud.Pendiente
            };

            SolicitudRutinaProfesor? solicitudActualizada = null;
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>()))
                .Callback<SolicitudRutinaProfesor>(s => solicitudActualizada = s)
                .Returns(Task.CompletedTask);

            // Act
            await _casoDeUso.EjecutarAsync(solicitudId);

            // Assert
            solicitudActualizada.Should().NotBeNull();
            solicitudActualizada!.Estado.Should().Be(EstadoSolicitud.Finalizada);
        }

        [Fact]
        public async Task EjecutarAsync_DebeAsignarFechaResolucion()
        {
            // Arrange
            var solicitudId = 1L;

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                Estado = EstadoSolicitud.TomadaPorProfesor,
                FechaResolucion = null
            };

            SolicitudRutinaProfesor? solicitudActualizada = null;
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>()))
                .Callback<SolicitudRutinaProfesor>(s => solicitudActualizada = s)
                .Returns(Task.CompletedTask);

            var tiempoAntes = DateTime.UtcNow;

            // Act
            await _casoDeUso.EjecutarAsync(solicitudId);

            var tiempoDespues = DateTime.UtcNow;

            // Assert
            solicitudActualizada.Should().NotBeNull();
            solicitudActualizada!.FechaResolucion.Should().NotBeNull();
            solicitudActualizada.FechaResolucion.Should().BeOnOrAfter(tiempoAntes);
            solicitudActualizada.FechaResolucion.Should().BeOnOrBefore(tiempoDespues);
        }

        [Fact]
        public async Task EjecutarAsync_DebeFinalizarSolicitudEnCualquierEstado()
        {
            // Arrange - Solicitud Pendiente
            var solicitud1 = new SolicitudRutinaProfesor
            {
                Id = 1,
                Estado = EstadoSolicitud.Pendiente
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(1L)).ReturnsAsync(solicitud1);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(1L);

            // Assert
            resultado.Should().BeTrue();
            solicitud1.Estado.Should().Be(EstadoSolicitud.Finalizada);
        }

        [Fact]
        public async Task EjecutarAsync_DebeActualizarEnRepositorio()
        {
            // Arrange
            var solicitudId = 1L;

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                Estado = EstadoSolicitud.TomadaPorProfesor
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>())).Returns(Task.CompletedTask);

            // Act
            await _casoDeUso.EjecutarAsync(solicitudId);

            // Assert
            _mockRepo.Verify(r => r.ActualizarAsync(It.Is<SolicitudRutinaProfesor>(s =>
                s.Id == solicitudId &&
                s.Estado == EstadoSolicitud.Finalizada
            )), Times.Once);
        }
    }
}
