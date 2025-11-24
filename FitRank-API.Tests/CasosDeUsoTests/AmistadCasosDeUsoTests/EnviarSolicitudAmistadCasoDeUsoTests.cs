using FitRank_API.Application.CasosDeUso.AmistadCasosDeUso;
using FitRank_API.Application.DTOs.AmistadDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.AmistadCasosDeUsoTests
{
    public class EnviarSolicitudAmistadCasoDeUsoTests
    {
        private readonly Mock<IAmistadRepositorio> _mockAmistadRepo;
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepo;
        private readonly EnviarSolicitudAmistadCasoDeUso _casoDeUso;

        public EnviarSolicitudAmistadCasoDeUsoTests()
        {
            _mockAmistadRepo = new Mock<IAmistadRepositorio>();
            _mockUsuarioRepo = new Mock<IUsuarioRepositorio>();
            _casoDeUso = new EnviarSolicitudAmistadCasoDeUso(_mockAmistadRepo.Object, _mockUsuarioRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRechazarSolicitudAMismoUsuario()
        {
            // Arrange
            var dto = new EnviarSolicitudAmistadDTO { SolicitanteId = 5, DestinatarioId = 5 };

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Completado.Should().BeFalse();
            resultado.Mensaje.Should().Contain("mismo");
        }

        [Fact]
        public async Task Ejecutar_DebeRechazarSiUsuarioNoExiste()
        {
            // Arrange
            var dto = new EnviarSolicitudAmistadDTO { SolicitanteId = 1, DestinatarioId = 2 };
            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(It.IsAny<long>())).ReturnsAsync((Usuario?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Completado.Should().BeFalse();
            resultado.Mensaje.Should().Contain("no existe");
        }

        [Fact]
        public async Task Ejecutar_DebeRechazarSiYaSonAmigos()
        {
            // Arrange
            var dto = new EnviarSolicitudAmistadDTO { SolicitanteId = 1, DestinatarioId = 2 };
            var solicitante = new Socio { Id = 1 };
            var destinatario = new Socio { Id = 2 };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(solicitante);
            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(2)).ReturnsAsync(destinatario);

            var amistadExistente = new Amistad { Estado = EstadoAmistad.Aceptado };
            _mockAmistadRepo.Setup(r => r.ObtenerPorIdDeSociosAsync(1, 2)).ReturnsAsync(amistadExistente);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Completado.Should().BeFalse();
            resultado.Mensaje.Should().Contain("amigos");
        }

        [Fact]
        public async Task Ejecutar_DebeRechazarSiYaExisteSolicitudPendiente()
        {
            // Arrange
            var dto = new EnviarSolicitudAmistadDTO { SolicitanteId = 1, DestinatarioId = 2 };
            var solicitante = new Socio { Id = 1 };
            var destinatario = new Socio { Id = 2 };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(solicitante);
            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(2)).ReturnsAsync(destinatario);

            var amistadExistente = new Amistad { Estado = EstadoAmistad.Pendiente };
            _mockAmistadRepo.Setup(r => r.ObtenerPorIdDeSociosAsync(1, 2)).ReturnsAsync(amistadExistente);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Completado.Should().BeFalse();
            resultado.Mensaje.Should().Contain("pendiente");
        }

        [Fact]
        public async Task Ejecutar_DebeCrearSolicitudCorrectamente()
        {
            // Arrange
            var dto = new EnviarSolicitudAmistadDTO { SolicitanteId = 3, DestinatarioId = 7 };
            var solicitante = new Socio { Id = 3 };
            var destinatario = new Socio { Id = 7 };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(3)).ReturnsAsync(solicitante);
            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(7)).ReturnsAsync(destinatario);
            _mockAmistadRepo.Setup(r => r.ObtenerPorIdDeSociosAsync(3, 7)).ReturnsAsync((Amistad?)null);

            var amistadCreada = new Amistad
            {
                Id = 1,
                SocioId1 = 3,
                SocioId2 = 7,
                SolicitanteId = 3,
                Estado = EstadoAmistad.Pendiente
            };
            _mockAmistadRepo.Setup(r => r.CrearAsync(It.IsAny<Amistad>())).ReturnsAsync(amistadCreada);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Completado.Should().BeTrue();
            resultado.Mensaje.Should().Contain("enviada");
            resultado.AmistadId.Should().Be(1);
            resultado.SolicitanteId.Should().Be(3);
            resultado.Estado.Should().Be("Pendiente");
        }

        [Fact]
        public async Task Ejecutar_DebeOrdenarCorrectamenteSocioId1YSocioId2()
        {
            // Arrange
            var dto = new EnviarSolicitudAmistadDTO { SolicitanteId = 10, DestinatarioId = 2 };
            var solicitante = new Socio { Id = 10 };
            var destinatario = new Socio { Id = 2 };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(10)).ReturnsAsync(solicitante);
            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(2)).ReturnsAsync(destinatario);
            _mockAmistadRepo.Setup(r => r.ObtenerPorIdDeSociosAsync(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync((Amistad?)null);

            var amistadCreada = new Amistad { Id = 1, SocioId1 = 2, SocioId2 = 10, SolicitanteId = 10 };
            _mockAmistadRepo.Setup(r => r.CrearAsync(It.IsAny<Amistad>())).ReturnsAsync(amistadCreada);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.SocioId1.Should().Be(2);
            resultado.SocioId2.Should().Be(10);
            _mockAmistadRepo.Verify(r => r.CrearAsync(It.Is<Amistad>(a => a.SocioId1 < a.SocioId2)), Times.Once);
        }
    }
}
