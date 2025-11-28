using FitRank_API.Application.CasosDeUso.SolicitudCasosDeUso;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.SolicitudCasosDeUsoTests
{
    public class TomarSolicitudCasoDeUsoTests
    {
        private readonly Mock<ISolicitudRutinaProfesorRepositorio> _mockRepo;
        private readonly TomarSolicitudCasoDeUso _casoDeUso;

        public TomarSolicitudCasoDeUsoTests()
        {
            _mockRepo = new Mock<ISolicitudRutinaProfesorRepositorio>();
            _casoDeUso = new TomarSolicitudCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task EjecutarAsync_DebeTomarSolicitudPendiente()
        {
            // Arrange
            var solicitudId = 1L;
            var profesorId = 5L;

            var solicitud = new SolicitudRutinaProfesor
            {
                Id = solicitudId,
                SocioId = 10,
                Estado = EstadoSolicitud.Pendiente,
                ProfesorId = null
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>())).Returns(Task.CompletedTask);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(solicitudId, profesorId);

            // Assert
            resultado.Should().BeTrue();
            solicitud.Estado.Should().Be(EstadoSolicitud.TomadaPorProfesor);
            solicitud.ProfesorId.Should().Be(profesorId);

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
            var resultado = await _casoDeUso.EjecutarAsync(solicitudId, profesorId);

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
                SocioId = 10,
                Estado = EstadoSolicitud.TomadaPorProfesor, // Ya está tomada
                ProfesorId = 3
            };

            _mockRepo.Setup(r => r.ObtenerPorIdAsync(solicitudId)).ReturnsAsync(solicitud);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(solicitudId, profesorId);

            // Assert
            resultado.Should().BeFalse();
            _mockRepo.Verify(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>()), Times.Never);
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
            var resultado = await _casoDeUso.EjecutarAsync(solicitudId, profesorId);

            // Assert
            resultado.Should().BeFalse();
            _mockRepo.Verify(r => r.ActualizarAsync(It.IsAny<SolicitudRutinaProfesor>()), Times.Never);
        }

        [Fact]
        public async Task EjecutarAsync_DebeRetornarFalseSiSolicitudRechazada()
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
            var resultado = await _casoDeUso.EjecutarAsync(solicitudId, profesorId);

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
            await _casoDeUso.EjecutarAsync(solicitudId, profesorId);

            // Assert
            solicitudActualizada.Should().NotBeNull();
            solicitudActualizada!.ProfesorId.Should().Be(profesorId);
        }

        [Fact]
        public async Task EjecutarAsync_DebeCambiarEstadoATomadaPorProfesor()
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
            await _casoDeUso.EjecutarAsync(solicitudId, profesorId);

            // Assert
            solicitudActualizada.Should().NotBeNull();
            solicitudActualizada!.Estado.Should().Be(EstadoSolicitud.TomadaPorProfesor);
        }
    }
}
