using FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso;
using FitRank_API.Application.Hubs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.NotificacionCasosDeUsoTests
{
    public class EnviarNotificacionMasivaCasoDeUsoTests
    {
        private readonly Mock<INotificacionRepositorio> _mockNotiRepo;
        private readonly Mock<IAdministradorRepositorio> _mockAdminRepo;
        private readonly Mock<IProfesorRepositorio> _mockProfRepo;
        private readonly Mock<ISocioRepositorio> _mockSocioRepo;
        private readonly Mock<IHubContext<NotificacionesHub>> _mockHub;
        private readonly Mock<IHubClients> _mockHubClients;
        private readonly Mock<IClientProxy> _mockClientProxy;
        private readonly EnviarNotificacionMasivaCasoDeUso _casoDeUso;

        public EnviarNotificacionMasivaCasoDeUsoTests()
        {
            _mockNotiRepo = new Mock<INotificacionRepositorio>();
            _mockAdminRepo = new Mock<IAdministradorRepositorio>();
            _mockProfRepo = new Mock<IProfesorRepositorio>();
            _mockSocioRepo = new Mock<ISocioRepositorio>();
            _mockHub = new Mock<IHubContext<NotificacionesHub>>();
            _mockHubClients = new Mock<IHubClients>();
            _mockClientProxy = new Mock<IClientProxy>();

            _mockHub.Setup(h => h.Clients).Returns(_mockHubClients.Object);
            _mockHubClients.Setup(c => c.Group(It.IsAny<string>())).Returns(_mockClientProxy.Object);

            _casoDeUso = new EnviarNotificacionMasivaCasoDeUso(
                _mockNotiRepo.Object,
                _mockAdminRepo.Object,
                _mockProfRepo.Object,
                _mockSocioRepo.Object,
                _mockHub.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeEnviarNotificacionATodosLosUsuariosDelGimnasio()
        {
            // Arrange
            var admin = new Administrador { Id = 1, GimnasioId = 10, Nombre = "Admin", Email = "admin@test.com" };

            _mockAdminRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(admin);
            _mockSocioRepo.Setup(r => r.ObtenerTodosPorGimnasio(10))
                .ReturnsAsync(new List<Socio>
                {
                    new Socio { Id = 2, GimnasioId = 10, Nombre = "Socio1", Email = "socio1@test.com" },
                    new Socio { Id = 3, GimnasioId = 10, Nombre = "Socio2", Email = "socio2@test.com" }
                });
            _mockProfRepo.Setup(r => r.ObtenerPorGimnasioAsync(10))
                .ReturnsAsync(new List<Profesor>
                {
                    new Profesor { Id = 4, GimnasioId = 10, Nombre = "Profesor1", Email = "prof1@test.com" }
                });
            _mockAdminRepo.Setup(r => r.ObtenerTodosPorGimnasio(10))
                .ReturnsAsync(new List<Administrador>
                {
                    new Administrador { Id = 5, GimnasioId = 10, Nombre = "Admin2", Email = "admin2@test.com" }
                });

            _mockNotiRepo.Setup(r => r.AgregarAsync(It.IsAny<Notificacion>()))
                .ReturnsAsync((Notificacion n) => { n.Id = 100; return n; });

            // Act
            var count = await _casoDeUso.Ejecutar(1, "Título Masivo", "Mensaje Masivo");

            // Assert
            count.Should().Be(4); // 2 socios + 1 profesor + 1 admin
            _mockNotiRepo.Verify(r => r.AgregarAsync(It.IsAny<Notificacion>()), Times.Exactly(4));
        }

        [Fact]
        public async Task Ejecutar_DebeLanzarExcepcionSiUsuarioNoTieneGimnasio()
        {
            // Arrange
            _mockAdminRepo.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Administrador?)null);
            _mockProfRepo.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Profesor?)null);
            _mockSocioRepo.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Socio?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => 
                _casoDeUso.Ejecutar(999, "Título", "Mensaje"));
        }

        [Fact]
        public async Task Ejecutar_DebeEnviarNotificacionPorSignalR()
        {
            // Arrange
            var admin = new Administrador { Id = 1, GimnasioId = 10, Nombre = "Admin", Email = "admin@test.com" };

            _mockAdminRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(admin);
            _mockSocioRepo.Setup(r => r.ObtenerTodosPorGimnasio(10))
                .ReturnsAsync(new List<Socio>
                {
                    new Socio { Id = 2, GimnasioId = 10, Nombre = "Socio1", Email = "socio1@test.com" }
                });
            _mockProfRepo.Setup(r => r.ObtenerPorGimnasioAsync(10))
                .ReturnsAsync(new List<Profesor>());
            _mockAdminRepo.Setup(r => r.ObtenerTodosPorGimnasio(10))
                .ReturnsAsync(new List<Administrador>());

            _mockNotiRepo.Setup(r => r.AgregarAsync(It.IsAny<Notificacion>()))
                .ReturnsAsync((Notificacion n) => { n.Id = 50; return n; });

            // Act
            await _casoDeUso.Ejecutar(1, "Test", "Mensaje");

            // Assert
            _mockHubClients.Verify(c => c.Group("user-2"), Times.Once);
            _mockClientProxy.Verify(
                c => c.SendCoreAsync(
                    "NotificacionRecibida",
                    It.IsAny<object[]>(),
                    default),
                Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarCeroCuandoNoHayUsuariosEnElGimnasio()
        {
            // Arrange
            var admin = new Administrador { Id = 1, GimnasioId = 10, Nombre = "Admin", Email = "admin@test.com" };

            _mockAdminRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(admin);
            _mockSocioRepo.Setup(r => r.ObtenerTodosPorGimnasio(10))
                .ReturnsAsync(new List<Socio>());
            _mockProfRepo.Setup(r => r.ObtenerPorGimnasioAsync(10))
                .ReturnsAsync(new List<Profesor>());
            _mockAdminRepo.Setup(r => r.ObtenerTodosPorGimnasio(10))
                .ReturnsAsync(new List<Administrador>());

            // Act
            var count = await _casoDeUso.Ejecutar(1, "Título", "Mensaje");

            // Assert
            count.Should().Be(0);
            _mockNotiRepo.Verify(r => r.AgregarAsync(It.IsAny<Notificacion>()), Times.Never);
        }

        [Fact]
        public async Task Ejecutar_DebeObtenerGimnasioDesdeProfesorSiNoEsAdmin()
        {
            // Arrange
            var profesor = new Profesor { Id = 10, GimnasioId = 20, Nombre = "Profe", Email = "prof@test.com" };

            _mockAdminRepo.Setup(r => r.ObtenerPorIdAsync(10)).ReturnsAsync((Administrador?)null);
            _mockProfRepo.Setup(r => r.ObtenerPorIdAsync(10)).ReturnsAsync(profesor);
            _mockSocioRepo.Setup(r => r.ObtenerTodosPorGimnasio(20))
                .ReturnsAsync(new List<Socio>
                {
                    new Socio { Id = 1, GimnasioId = 20, Nombre = "Socio", Email = "socio@test.com" }
                });
            _mockProfRepo.Setup(r => r.ObtenerPorGimnasioAsync(20))
                .ReturnsAsync(new List<Profesor>());
            _mockAdminRepo.Setup(r => r.ObtenerTodosPorGimnasio(20))
                .ReturnsAsync(new List<Administrador>());

            _mockNotiRepo.Setup(r => r.AgregarAsync(It.IsAny<Notificacion>()))
                .ReturnsAsync((Notificacion n) => { n.Id = 1; return n; });

            // Act
            var count = await _casoDeUso.Ejecutar(10, "Título", "Mensaje");

            // Assert
            count.Should().Be(1);
        }
    }
}
