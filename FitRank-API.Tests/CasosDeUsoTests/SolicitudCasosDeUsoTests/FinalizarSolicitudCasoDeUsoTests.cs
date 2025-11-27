using FitRank_API.Application.CasosDeUso.SolicitudCasosDeUso;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.SolicitudCasosDeUsoTests
{
    public class FinalizarSolicitudCasoDeUsoTests
    {
        private readonly Mock<ISolicitudRutinaProfesorRepositorio> _mockRepo;
        private readonly FinalizarSolicitudCasoDeUso _casoDeUso;

        public FinalizarSolicitudCasoDeUsoTests()
        {
            _mockRepo = new Mock<ISolicitudRutinaProfesorRepositorio>();
            _casoDeUso = new FinalizarSolicitudCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task EjecutarAsync_DebeFinalizarSolicitudTomada()
        {
            // Arrange
            var solicitudId = 1L;
            var rutinaId = 10L;
            var mensaje = "Rutina creada exitosamente para el socio";

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                Estado = EstadoSolicitud.TomadaPorProfesor,
                ProfesorId = 5,
                RutinaId = null,
                MensajeProfesor = null,
                FechaResolucion = null
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(solicitudId, rutinaId, mensaje);

            // Assert
            resultado.Should().BeTrue();
            solicitud.Estado.Should().Be(EstadoSolicitud.Finalizada);
            solicitud.RutinaId.Should().Be(rutinaId);
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
            var rutinaId = 10L;

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync((SolicitudRutinaProfesor?)null);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(solicitudId, rutinaId, "mensaje");

            // Assert
            resultado.Should().BeFalse();
            _mockRepo.Verify(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_DebeRetornarFalseSiSolicitudNoEstaTomada()
        {
            // Arrange
            var solicitudId = 1L;
            var rutinaId = 10L;

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                Estado = EstadoSolicitud.Pendiente // No está tomada
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(solicitudId, rutinaId, "mensaje");

            // Assert
            resultado.Should().BeFalse();
            _mockRepo.Verify(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_DebeRetornarFalseSiSolicitudYaFinalizada()
        {
            // Arrange
            var solicitudId = 1L;
            var rutinaId = 10L;

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                Estado = EstadoSolicitud.Finalizada
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(solicitudId, rutinaId, "mensaje");

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task EjecutarAsync_DebeRetornarFalseSiSolicitudRechazada()
        {
            // Arrange
            var solicitudId = 1L;
            var rutinaId = 10L;

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                Estado = EstadoSolicitud.Rechazada
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(solicitudId, rutinaId, "mensaje");

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task EjecutarAsync_DebeAsignarRutinaId()
        {
            // Arrange
            var solicitudId = 1L;
            var rutinaId = 25L;

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                Estado = EstadoSolicitud.TomadaPorProfesor,
                RutinaId = null
            };

            SolicitudRutinaProfesor? solicitudActualizada = null;
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>()))
                .Callback<SolicitudRutinaProfesor>(s => solicitudActualizada = s)
                .Returns(Task.CompletedTask);

            // Act
            await _casoDeUso.EjecutarAsync(solicitudId, rutinaId, "mensaje");

            // Assert
            solicitudActualizada.Should().NotBeNull();
            solicitudActualizada!.RutinaId.Should().Be(rutinaId);
        }

        [Fact]
        public async Task EjecutarAsync_DebeGuardarMensajeDelProfesor()
        {
            // Arrange
            var solicitudId = 1L;
            var rutinaId = 10L;
            var mensaje = "Rutina personalizada completada. Seguir las indicaciones adjuntas.";

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                Estado = EstadoSolicitud.TomadaPorProfesor
            };

            SolicitudRutinaProfesor? solicitudActualizada = null;
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>()))
                .Callback<SolicitudRutinaProfesor>(s => solicitudActualizada = s)
                .Returns(Task.CompletedTask);

            // Act
            await _casoDeUso.EjecutarAsync(solicitudId, rutinaId, mensaje);

            // Assert
            solicitudActualizada.Should().NotBeNull();
            solicitudActualizada!.MensajeProfesor.Should().Be(mensaje);
        }

        [Fact]
        public async Task EjecutarAsync_DebePermitirMensajeNull()
        {
            // Arrange
            var solicitudId = 1L;
            var rutinaId = 10L;

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                Estado = EstadoSolicitud.TomadaPorProfesor
            };

            SolicitudRutinaProfesor? solicitudActualizada = null;
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>()))
                .Callback<SolicitudRutinaProfesor>(s => solicitudActualizada = s)
                .Returns(Task.CompletedTask);

            // Act
            await _casoDeUso.EjecutarAsync(solicitudId, rutinaId, null);

            // Assert
            solicitudActualizada.Should().NotBeNull();
            solicitudActualizada!.MensajeProfesor.Should().BeNull();
        }

        [Fact]
        public async Task EjecutarAsync_DebeAsignarFechaResolucion()
        {
            // Arrange
            var solicitudId = 1L;
            var rutinaId = 10L;

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
            await _casoDeUso.EjecutarAsync(solicitudId, rutinaId, "mensaje");

            var tiempoDespues = DateTime.UtcNow;

            // Assert
            solicitudActualizada.Should().NotBeNull();
            solicitudActualizada!.FechaResolucion.Should().NotBeNull();
            solicitudActualizada.FechaResolucion.Should().BeOnOrAfter(tiempoAntes);
            solicitudActualizada.FechaResolucion.Should().BeOnOrBefore(tiempoDespues);
        }

        [Fact]
        public async Task EjecutarAsync_DebeCambiarEstadoAFinalizada()
        {
            // Arrange
            var solicitudId = 1L;
            var rutinaId = 10L;

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                Estado = EstadoSolicitud.TomadaPorProfesor
            };

            SolicitudRutinaProfesor? solicitudActualizada = null;
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>()))
                .Callback<SolicitudRutinaProfesor>(s => solicitudActualizada = s)
                .Returns(Task.CompletedTask);

            // Act
            await _casoDeUso.EjecutarAsync(solicitudId, rutinaId, "mensaje");

            // Assert
            solicitudActualizada.Should().NotBeNull();
            solicitudActualizada!.Estado.Should().Be(EstadoSolicitud.Finalizada);
        }
    }
}
