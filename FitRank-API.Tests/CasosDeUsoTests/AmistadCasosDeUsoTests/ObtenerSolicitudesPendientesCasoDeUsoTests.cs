using FitRank_API.Application.CasosDeUso.AmistadCasosDeUso;
using FitRank_API.Application.DTOs.AmistadDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.AmistadCasosDeUsoTests
{
    public class ObtenerSolicitudesPendientesCasoDeUsoTests
    {
        private readonly Mock<IAmistadRepositorio> _mockRepo;
        private readonly ObtenerSolicitudesPendientesCasoDeUso _casoDeUso;

        public ObtenerSolicitudesPendientesCasoDeUsoTests()
        {
            _mockRepo = new Mock<IAmistadRepositorio>();
            _casoDeUso = new ObtenerSolicitudesPendientesCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarListaVaciaSiNoHaySolicitudesPendientes()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObtenerSolicitudesPendientesAsync(5))
                .ReturnsAsync(new List<Amistad>());

            // Act
            var resultado = await _casoDeUso.Ejecutar(5);

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarSolicitudesPendientesCorrectamente()
        {
            // Arrange
            var solicitante1 = new Socio { Id = 10, NombreUsuario = "user10", Nombre = "Juan", Puntaje = 100 };
            var solicitante2 = new Socio { Id = 15, NombreUsuario = "user15", Nombre = "Pedro", Puntaje = 200 };

            var solicitudes = new List<Amistad>
            {
                new Amistad { Id = 1, SolicitanteId = 10, Solicitante = solicitante1 },
                new Amistad { Id = 2, SolicitanteId = 15, Solicitante = solicitante2 }
            };

            _mockRepo.Setup(r => r.ObtenerSolicitudesPendientesAsync(5))
                .ReturnsAsync(solicitudes);

            // Act
            var resultado = await _casoDeUso.Ejecutar(5);

            // Assert
            resultado.Should().HaveCount(2);
            resultado.Should().Contain(s => s.RemitenteId == 10 && s.RemitenteNombreUsuario == "user10");
            resultado.Should().Contain(s => s.RemitenteId == 15 && s.RemitentePuntaje == 200);
        }

        [Fact]
        public async Task Ejecutar_DebeMapearCorrectamenteDatosDelSolicitante()
        {
            // Arrange
            var solicitante = new Socio
            {
                Id = 7,
                NombreUsuario = "testuser",
                Nombre = "TestName",
                Puntaje = 500
            };

            var solicitudes = new List<Amistad>
            {
                new Amistad { Id = 10, SolicitanteId = 7, Solicitante = solicitante }
            };

            _mockRepo.Setup(r => r.ObtenerSolicitudesPendientesAsync(3))
                .ReturnsAsync(solicitudes);

            // Act
            var resultado = await _casoDeUso.Ejecutar(3);

            // Assert
            resultado.Should().HaveCount(1);
            var solicitud = resultado.First();
            solicitud.AmistadId.Should().Be(10);
            solicitud.RemitenteId.Should().Be(7);
            solicitud.RemitenteNombreUsuario.Should().Be("testuser");
            solicitud.RemitenteNombre.Should().Be("TestName");
            solicitud.RemitentePuntaje.Should().Be(500);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarPuntajeCeroSiSolicitanteNoEsSocio()
        {
            // Arrange
            var solicitante = new Socio { Id = 8, NombreUsuario = "user8", Nombre = "Usuario", Puntaje = 0 };

            var solicitudes = new List<Amistad>
            {
                new Amistad { Id = 1, SolicitanteId = 8, Solicitante = solicitante }
            };

            _mockRepo.Setup(r => r.ObtenerSolicitudesPendientesAsync(5))
                .ReturnsAsync(solicitudes);

            // Act
            var resultado = await _casoDeUso.Ejecutar(5);

            // Assert
            resultado.Should().HaveCount(1);
            resultado.First().RemitentePuntaje.Should().Be(0);
        }
    }
}
