using FitRank_API.Application.CasosDeUso.AmistadCasosDeUso;
using FitRank_API.Application.DTOs.AmistadDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.AmistadCasosDeUsoTests
{
    public class AceptarSolicitudAmistadCasoDeUsoTests
    {
        private readonly Mock<IAmistadRepositorio> _mockRepo;
        private readonly AceptarSolicitudAmistadCasoDeUso _casoDeUso;

        public AceptarSolicitudAmistadCasoDeUsoTests()
        {
            _mockRepo = new Mock<IAmistadRepositorio>();
            _casoDeUso = new AceptarSolicitudAmistadCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRechazarSiSolicitudNoExiste()
        {
            // Arrange
            var dto = new AceptarSolicitudAmistadDTO { AmistadId = 1, SocioId = 5 };
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync((Amistad?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Completado.Should().BeFalse();
            resultado.Mensaje.Should().Contain("no encontrada");
        }

        [Fact]
        public async Task Ejecutar_DebeRechazarSiSocioNoEsParte()
        {
            // Arrange
            var dto = new AceptarSolicitudAmistadDTO { AmistadId = 1, SocioId = 99 };
            var amistad = new Amistad { Id = 1, SocioId1 = 5, SocioId2 = 10, Estado = EstadoAmistad.Pendiente };
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(amistad);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Completado.Should().BeFalse();
            resultado.Mensaje.Should().Contain("No podés");
        }

        [Fact]
        public async Task Ejecutar_DebeRechazarSiSolicitudNoEstaPendiente()
        {
            // Arrange
            var dto = new AceptarSolicitudAmistadDTO { AmistadId = 1, SocioId = 5 };
            var amistad = new Amistad { Id = 1, SocioId1 = 5, SocioId2 = 10, Estado = EstadoAmistad.Aceptado };
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(amistad);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Completado.Should().BeFalse();
            resultado.Mensaje.Should().Contain("no está pendiente");
        }

        [Fact]
        public async Task Ejecutar_DebeRechazarSiSolicitanteIntentaAceptar()
        {
            // Arrange
            var dto = new AceptarSolicitudAmistadDTO { AmistadId = 1, SocioId = 5 };
            var amistad = new Amistad
            {
                Id = 1,
                SocioId1 = 5,
                SocioId2 = 10,
                SolicitanteId = 5,
                Estado = EstadoAmistad.Pendiente
            };
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(amistad);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Completado.Should().BeFalse();
            resultado.Mensaje.Should().Contain("propia solicitud");
        }

        [Fact]
        public async Task Ejecutar_DebeAceptarSolicitudCorrectamente()
        {
            // Arrange
            var dto = new AceptarSolicitudAmistadDTO { AmistadId = 1, SocioId = 10 };
            var amistad = new Amistad
            {
                Id = 1,
                SocioId1 = 5,
                SocioId2 = 10,
                SolicitanteId = 5,
                Estado = EstadoAmistad.Pendiente
            };
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(amistad);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<Amistad>())).ReturnsAsync(amistad);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Completado.Should().BeTrue();
            resultado.Mensaje.Should().Contain("aceptada");
            resultado.Estado.Should().Be("Aceptado");
            amistad.Estado.Should().Be(EstadoAmistad.Aceptado);
        }

        [Fact]
        public async Task Ejecutar_DebeActualizarFechaActualizacion()
        {
            // Arrange
            var dto = new AceptarSolicitudAmistadDTO { AmistadId = 1, SocioId = 10 };
            var fechaAnterior = DateTime.UtcNow.AddDays(-1);
            var amistad = new Amistad
            {
                Id = 1,
                SocioId1 = 5,
                SocioId2 = 10,
                SolicitanteId = 5,
                Estado = EstadoAmistad.Pendiente,
                FechaActualizacion = fechaAnterior
            };
            _mockRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(amistad);
            _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<Amistad>())).ReturnsAsync(amistad);

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            amistad.FechaActualizacion.Should().BeAfter(fechaAnterior);
        }
    }
}
