using FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso;
using FitRank_API.Application.DTOs.NotificacionDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.NotificacionCasosDeUsoTests
{
    public class ObtenerHistorialNotificacionesCasoDeUsoTests
    {
        private readonly Mock<INotificacionRepositorio> _mockNotiRepo;
        private readonly Mock<IAdministradorRepositorio> _mockAdminRepo;
        private readonly Mock<IProfesorRepositorio> _mockProfRepo;
        private readonly Mock<ISocioRepositorio> _mockSocioRepo;
        private readonly ObtenerHistorialNotificacionesCasoDeUso _casoDeUso;

        public ObtenerHistorialNotificacionesCasoDeUsoTests()
        {
            _mockNotiRepo = new Mock<INotificacionRepositorio>();
            _mockAdminRepo = new Mock<IAdministradorRepositorio>();
            _mockProfRepo = new Mock<IProfesorRepositorio>();
            _mockSocioRepo = new Mock<ISocioRepositorio>();

            _casoDeUso = new ObtenerHistorialNotificacionesCasoDeUso(
                _mockNotiRepo.Object,
                _mockAdminRepo.Object,
                _mockProfRepo.Object,
                _mockSocioRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarHistorialDeNotificacionesDelGimnasio()
        {
            // Arrange
            var admin = new Administrador { Id = 1, GimnasioId = 10, Nombre = "Admin", Email = "admin@test.com" };
            var socio1 = new Socio { Id = 2, GimnasioId = 10, Nombre = "Socio1", Email = "socio1@test.com" };
            var socio2 = new Socio { Id = 3, GimnasioId = 10, Nombre = "Socio2", Email = "socio2@test.com" };

            _mockAdminRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(admin);
            _mockSocioRepo.Setup(r => r.ObtenerTodosPorGimnasio(10))
                .ReturnsAsync(new List<Socio> { socio1, socio2 });
            _mockProfRepo.Setup(r => r.ObtenerPorGimnasioAsync(10))
                .ReturnsAsync(new List<Profesor>());
            _mockAdminRepo.Setup(r => r.ObtenerTodosPorGimnasio(10))
                .ReturnsAsync(new List<Administrador> { admin });

            var notificaciones = new List<Notificacion>
            {
                new Notificacion
                {
                    Id = 1,
                    UsuarioEmisorId = 1,
                    UsuarioReceptorId = 2,
                    Titulo = "Notificación 1",
                    Mensaje = "Mensaje 1",
                    FechaEnvio = DateTime.UtcNow,
                    UsuarioEmisor = new Usuario { Id = 1, Nombre = "Admin" },
                    UsuarioReceptor = new Usuario { Id = 2, Nombre = "Socio1" }
                },
                new Notificacion
                {
                    Id = 2,
                    UsuarioEmisorId = 1,
                    UsuarioReceptorId = 3,
                    Titulo = "Notificación 2",
                    Mensaje = "Mensaje 2",
                    FechaEnvio = DateTime.UtcNow.AddHours(-1),
                    UsuarioEmisor = new Usuario { Id = 1, Nombre = "Admin" },
                    UsuarioReceptor = new Usuario { Id = 3, Nombre = "Socio2" }
                }
            };

            _mockNotiRepo.Setup(r => r.ObtenerNotificacionesDelGimnasio(It.IsAny<List<long>>()))
                .ReturnsAsync(notificaciones);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.First().Titulo.Should().Be("Notificación 1");
            resultado.Last().Titulo.Should().Be("Notificación 2");
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarListaVaciaSiUsuarioNoTieneGimnasio()
        {
            // Arrange
            _mockAdminRepo.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Administrador?)null);
            _mockProfRepo.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Profesor?)null);
            _mockSocioRepo.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Socio?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(999);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeMappearCorrectamenteEmisorYReceptor()
        {
            // Arrange
            var admin = new Administrador { Id = 1, GimnasioId = 10, Nombre = "Admin", Email = "admin@test.com" };

            _mockAdminRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(admin);
            _mockSocioRepo.Setup(r => r.ObtenerTodosPorGimnasio(10))
                .ReturnsAsync(new List<Socio>());
            _mockProfRepo.Setup(r => r.ObtenerPorGimnasioAsync(10))
                .ReturnsAsync(new List<Profesor>());
            _mockAdminRepo.Setup(r => r.ObtenerTodosPorGimnasio(10))
                .ReturnsAsync(new List<Administrador> { admin });

            var notificaciones = new List<Notificacion>
            {
                new Notificacion
                {
                    Id = 1,
                    UsuarioEmisorId = 1,
                    UsuarioReceptorId = 2,
                    Titulo = "Test",
                    Mensaje = "Test",
                    FechaEnvio = DateTime.UtcNow,
                    UsuarioEmisor = new Usuario { Id = 1, Nombre = "EmisorNombre" },
                    UsuarioReceptor = new Usuario { Id = 2, Nombre = "ReceptorNombre" }
                }
            };

            _mockNotiRepo.Setup(r => r.ObtenerNotificacionesDelGimnasio(It.IsAny<List<long>>()))
                .ReturnsAsync(notificaciones);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            var notificacion = resultado.First();
            notificacion.Emisor.Should().Be("EmisorNombre");
            notificacion.Receptor.Should().Be("ReceptorNombre");
        }

        [Fact]
        public async Task Ejecutar_DebeUsarDesconocidoCuandoUsuarioEsNull()
        {
            // Arrange
            var admin = new Administrador { Id = 1, GimnasioId = 10, Nombre = "Admin", Email = "admin@test.com" };

            _mockAdminRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(admin);
            _mockSocioRepo.Setup(r => r.ObtenerTodosPorGimnasio(10))
                .ReturnsAsync(new List<Socio>());
            _mockProfRepo.Setup(r => r.ObtenerPorGimnasioAsync(10))
                .ReturnsAsync(new List<Profesor>());
            _mockAdminRepo.Setup(r => r.ObtenerTodosPorGimnasio(10))
                .ReturnsAsync(new List<Administrador> { admin });

            var notificaciones = new List<Notificacion>
            {
                new Notificacion
                {
                    Id = 1,
                    UsuarioEmisorId = 1,
                    UsuarioReceptorId = 2,
                    Titulo = "Test",
                    Mensaje = "Test",
                    FechaEnvio = DateTime.UtcNow,
                    UsuarioEmisor = null,
                    UsuarioReceptor = null
                }
            };

            _mockNotiRepo.Setup(r => r.ObtenerNotificacionesDelGimnasio(It.IsAny<List<long>>()))
                .ReturnsAsync(notificaciones);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            var notificacion = resultado.First();
            notificacion.Emisor.Should().Be("Desconocido");
            notificacion.Receptor.Should().Be("Desconocido");
        }

        [Fact]
        public async Task Ejecutar_DebeObtenerGimnasioDesdeProfesorSiNoEsAdmin()
        {
            // Arrange
            var profesor = new Profesor { Id = 5, GimnasioId = 20, Nombre = "Profesor", Email = "prof@test.com" };

            _mockAdminRepo.Setup(r => r.ObtenerPorIdAsync(5)).ReturnsAsync((Administrador?)null);
            _mockProfRepo.Setup(r => r.ObtenerPorIdAsync(5)).ReturnsAsync(profesor);
            _mockSocioRepo.Setup(r => r.ObtenerTodosPorGimnasio(20))
                .ReturnsAsync(new List<Socio>());
            _mockProfRepo.Setup(r => r.ObtenerPorGimnasioAsync(20))
                .ReturnsAsync(new List<Profesor> { profesor });
            _mockAdminRepo.Setup(r => r.ObtenerTodosPorGimnasio(20))
                .ReturnsAsync(new List<Administrador>());

            _mockNotiRepo.Setup(r => r.ObtenerNotificacionesDelGimnasio(It.IsAny<List<long>>()))
                .ReturnsAsync(new List<Notificacion>());

            // Act
            var resultado = await _casoDeUso.Ejecutar(5);

            // Assert
            resultado.Should().NotBeNull();
            _mockProfRepo.Verify(r => r.ObtenerPorGimnasioAsync(20), Times.Once);
        }
    }
}
