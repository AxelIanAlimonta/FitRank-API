using FitRank_API.Application.CasosDeUso.SolicitudCasosDeUso;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.SolicitudCasosDeUsoTests
{
    public class RechazarSolicitudCasoDeUsoTests
    {
        private readonly Mock<ISolicitudRutinaProfesorRepositorio> _mockRepo;
        private readonly RechazarSolicitudCasoDeUso _casoDeUso;

        public RechazarSolicitudCasoDeUsoTests()
        {
            _mockRepo = new Mock<ISolicitudRutinaProfesorRepositorio>();
            _casoDeUso = new RechazarSolicitudCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task EjecutarAsync_DebeRechazarSolicitudPendiente()
        {
            // Arrange
            var solicitudId = 1L;
            var profesorId = 5L;
            var mensaje = "No puedo atender esta solicitud en este momento";

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                Estado = EstadoSolicitud.Pendiente,
                ProfesorId = null,
                MensajeProfesor = null,
                FechaResolucion = null
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(solicitudId, profesorId, mensaje);

            // Assert
            resultado.Should().BeTrue();
            solicitud.Estado.Should().Be(EstadoSolicitud.Rechazada);
            solicitud.ProfesorId.Should().Be(profesorId);
            solicitud.MensajeProfesor.Should().Be(mensaje);
            solicitud.FechaResolucion.Should().NotBeNull();
            solicitud.FechaResolucion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            _mockRepo.Verify(r => r.ActualizarAsync(solicitud), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_DebeRetornarFalseSiSolicitudNoExiste()
        {
            // Arrange
            var solicitudId = 999L;
            var profesorId = 5L;

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync((SolicitudRutinaProfesor?)null);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(solicitudId, profesorId, "mensaje");

            // Assert
            resultado.Should().BeFalse();
            _mockRepo.Verify(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_DebeRetornarFalseSiSolicitudNoEstaPendiente()
        {
            // Arrange
            var solicitudId = 1L;
            var profesorId = 5L;

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                Estado = EstadoSolicitud.TomadaPorProfesor
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(solicitudId, profesorId, "mensaje");

            // Assert
            resultado.Should().BeFalse();
            _mockRepo.Verify(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_DebeRetornarFalseSiSolicitudYaRechazada()
        {
            // Arrange
            var solicitudId = 1L;
            var profesorId = 5L;

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                Estado = EstadoSolicitud.Rechazada
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(solicitudId, profesorId, "mensaje");

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task EjecutarAsync_DebeRetornarFalseSiSolicitudFinalizada()
        {
            // Arrange
            var solicitudId = 1L;
            var profesorId = 5L;

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                Estado = EstadoSolicitud.Finalizada
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(solicitudId, profesorId, "mensaje");

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task EjecutarAsync_DebeAsignarProfesorCorrectamente()
        {
            // Arrange
            var solicitudId = 1L;
            var profesorId = 15L;

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                Estado = EstadoSolicitud.Pendiente,
                ProfesorId = null
            };

            SolicitudRutinaProfesor? solicitudActualizada = null;
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>()))
                .Callback<SolicitudRutinaProfesor>(s => solicitudActualizada = s)
                .Returns(Task.CompletedTask);

            // Act
            await _casoDeUso.EjecutarAsync(solicitudId, profesorId, "mensaje");

            // Assert
            solicitudActualizada.Should().NotBeNull();
            solicitudActualizada!.ProfesorId.Should().Be(profesorId);
        }

        [Fact]
        public async Task EjecutarAsync_DebeGuardarMensajeDelProfesor()
        {
            // Arrange
            var solicitudId = 1L;
            var profesorId = 5L;
            var mensaje = "Motivo del rechazo: carga de trabajo completa";

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
            await _casoDeUso.EjecutarAsync(solicitudId, profesorId, mensaje);

            // Assert
            solicitudActualizada.Should().NotBeNull();
            solicitudActualizada!.MensajeProfesor.Should().Be(mensaje);
        }

        [Fact]
        public async Task EjecutarAsync_DebePermitirMensajeNull()
        {
            // Arrange
            var solicitudId = 1L;
            var profesorId = 5L;

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
            await _casoDeUso.EjecutarAsync(solicitudId, profesorId, null);

            // Assert
            solicitudActualizada.Should().NotBeNull();
            solicitudActualizada!.MensajeProfesor.Should().BeNull();
        }

        [Fact]
        public async Task EjecutarAsync_DebeAsignarFechaResolucion()
        {
            // Arrange
            var solicitudId = 1L;
            var profesorId = 5L;

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                Estado = EstadoSolicitud.Pendiente,
                FechaResolucion = null
            };

            SolicitudRutinaProfesor? solicitudActualizada = null;
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>()))
                .Callback<SolicitudRutinaProfesor>(s => solicitudActualizada = s)
                .Returns(Task.CompletedTask);

            var tiempoAntes = DateTime.UtcNow;

            // Act
            await _casoDeUso.EjecutarAsync(solicitudId, profesorId, "mensaje");

            var tiempoDespues = DateTime.UtcNow;

            // Assert
            solicitudActualizada.Should().NotBeNull();
            solicitudActualizada!.FechaResolucion.Should().NotBeNull();
            solicitudActualizada.FechaResolucion.Should().BeOnOrAfter(tiempoAntes);
            solicitudActualizada.FechaResolucion.Should().BeOnOrBefore(tiempoDespues);
        }
    }
}
