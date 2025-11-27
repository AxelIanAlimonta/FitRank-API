using AutoMapper;
using FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso;
using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Application.Helpers;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text.Json;

namespace FitRank_API.Tests.CasosDeUsoTests.UsuarioCasosDeUsoTests
{
    public class AgregarUsuarioConInvitacionCasoDeUsoTests
    {
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepo;
        private readonly Mock<IInvitacionRepositorio> _mockInvitacionRepo;
        private readonly Mock<GenerarTokenCasoDeUso> _mockGenerarToken;
        private readonly IMapper _mapper;
        private readonly AgregarUsuarioConInvitacionCasoDeUso _casoDeUso;

        public AgregarUsuarioConInvitacionCasoDeUsoTests()
        {
            _mockUsuarioRepo = new Mock<IUsuarioRepositorio>();
            _mockInvitacionRepo = new Mock<IInvitacionRepositorio>();
            _mockGenerarToken = new Mock<GenerarTokenCasoDeUso>(MockBehavior.Loose, It.IsAny<IConfiguration>(), It.IsAny<IUsuarioRepositorio>());

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Usuario, UsuarioAuthDTO>();
            });
            _mapper = config.CreateMapper();

            _casoDeUso = new AgregarUsuarioConInvitacionCasoDeUso(
                _mockUsuarioRepo.Object,
                _mockInvitacionRepo.Object,
                _mapper,
                _mockGenerarToken.Object
            );
        }

        [Fact]
        public async Task Ejecutar_DebeRegistrarSocioConInvitacionValida()
        {
            // Arrange
            var invitacion = new Invitacion
            {
                Id = 1,
                Email = "socio@test.com",
                Estado = "Pendiente",
                UsuarioId = null,
                GimnasioId = 5,
                CuotaPagadaHasta = DateTime.UtcNow.AddMonths(1),
                DatosPrellenados = JsonSerializer.Serialize(new Dictionary<string, object>
                {
                    { "nombre", "Juan" },
                    { "apellidos", "Pérez" },
                    { "dni", "12345678" },
                    { "telefono", "123456789" }
                })
            };

            var dto = new RegisterInvitacionDTO
            {
                TokenInvitacion = $"simple_token_{invitacion.Id}",
                NombreUsuario = "juanperez",
                Password = "Password123!",
                FechaNacimiento = new DateTime(1990, 1, 1)
            };

            _mockInvitacionRepo.Setup(r => r.ObtenerPorIdAsync(invitacion.Id)).ReturnsAsync(invitacion);
            _mockInvitacionRepo.Setup(r => r.ActualizarAsync(It.IsAny<Invitacion>())).ReturnsAsync((Invitacion i) => i);
            _mockUsuarioRepo.Setup(r => r.AgregarAsync(It.IsAny<Usuario>()))
                .ReturnsAsync((Usuario u) => { u.Id = 10; return u; });
            _mockGenerarToken.Setup(g => g.Ejecutar(It.IsAny<Usuario>())).Returns("token_generado");

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Token.Should().Be("token_generado");
            resultado.User.Should().NotBeNull();

            _mockUsuarioRepo.Verify(r => r.AgregarAsync(It.Is<Usuario>(s =>
                s.Email == invitacion.Email &&
                s.NombreUsuario == dto.NombreUsuario &&
                ((Socio)s).GimnasioId == invitacion.GimnasioId &&
                s.EsActivado == true &&
                s.Rol == "User"
            )), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarNullSiTokenInvalido()
        {
            // Arrange
            var dto = new RegisterInvitacionDTO
            {
                TokenInvitacion = "token_invalido",
                NombreUsuario = "usuario",
                Password = "Password123!",
                FechaNacimiento = new DateTime(1990, 1, 1)
            };

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().BeNull();
            _mockInvitacionRepo.Verify(r => r.ObtenerPorIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarNullSiInvitacionNoExiste()
        {
            // Arrange
            var dto = new RegisterInvitacionDTO
            {
                TokenInvitacion = "simple_token_999",
                NombreUsuario = "usuario",
                Password = "Password123!",
                FechaNacimiento = new DateTime(1990, 1, 1)
            };

            _mockInvitacionRepo.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((Invitacion?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().BeNull();
            _mockUsuarioRepo.Verify(r => r.AgregarAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarNullSiInvitacionYaUsada()
        {
            // Arrange
            var invitacion = new Invitacion
            {
                Id = 1,
                Email = "socio@test.com",
                Estado = "Usada",
                UsuarioId = 50, // Ya tiene usuario asignado
                GimnasioId = 5,
                DatosPrellenados = "{}"
            };

            var dto = new RegisterInvitacionDTO
            {
                TokenInvitacion = "simple_token_1",
                NombreUsuario = "usuario",
                Password = "Password123!",
                FechaNacimiento = new DateTime(1990, 1, 1)
            };

            _mockInvitacionRepo.Setup(r => r.ObtenerPorIdAsync(invitacion.Id)).ReturnsAsync(invitacion);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().BeNull();
            _mockUsuarioRepo.Verify(r => r.AgregarAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task Ejecutar_DebeActualizarInvitacionComoUsada()
        {
            // Arrange
            var invitacion = new Invitacion
            {
                Id = 1,
                Email = "socio@test.com",
                Estado = "Pendiente",
                UsuarioId = null,
                GimnasioId = 5,
                DatosPrellenados = "{}"
            };

            var dto = new RegisterInvitacionDTO
            {
                TokenInvitacion = "simple_token_1",
                NombreUsuario = "usuario",
                Password = "Password123!",
                FechaNacimiento = new DateTime(1990, 1, 1)
            };

            _mockInvitacionRepo.Setup(r => r.ObtenerPorIdAsync(invitacion.Id)).ReturnsAsync(invitacion);
            _mockInvitacionRepo.Setup(r => r.ActualizarAsync(It.IsAny<Invitacion>())).ReturnsAsync((Invitacion i) => i);
            _mockUsuarioRepo.Setup(r => r.AgregarAsync(It.IsAny<Usuario>()))
                .ReturnsAsync((Usuario u) => { u.Id = 10; return u; });
            _mockGenerarToken.Setup(g => g.Ejecutar(It.IsAny<Usuario>())).Returns("token");

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            _mockInvitacionRepo.Verify(r => r.ActualizarAsync(It.Is<Invitacion>(i =>
                i.Estado == "Usada" &&
                i.UsuarioId == 10
            )), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeHashearPasswordCorrectamente()
        {
            // Arrange
            var invitacion = new Invitacion
            {
                Id = 1,
                Email = "socio@test.com",
                Estado = "Pendiente",
                UsuarioId = null,
                GimnasioId = 5,
                DatosPrellenados = "{}"
            };

            var dto = new RegisterInvitacionDTO
            {
                TokenInvitacion = "simple_token_1",
                NombreUsuario = "usuario",
                Password = "MiPasswordSegura123!",
                FechaNacimiento = new DateTime(1990, 1, 1)
            };

            Socio? socioGuardado = null;
            _mockInvitacionRepo.Setup(r => r.ObtenerPorIdAsync(invitacion.Id)).ReturnsAsync(invitacion);
            _mockInvitacionRepo.Setup(r => r.ActualizarAsync(It.IsAny<Invitacion>())).ReturnsAsync((Invitacion i) => i);
            _mockUsuarioRepo.Setup(r => r.AgregarAsync(It.IsAny<Usuario>()))
                .ReturnsAsync((Usuario u) => { u.Id = 10; socioGuardado = u as Socio; return u; });
            _mockGenerarToken.Setup(g => g.Ejecutar(It.IsAny<Usuario>())).Returns("token");

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            socioGuardado.Should().NotBeNull();
            socioGuardado!.PasswordHash.Should().NotBe(dto.Password);
            BCrypt.Net.BCrypt.Verify(dto.Password, socioGuardado.PasswordHash).Should().BeTrue();
        }

        [Fact]
        public async Task Ejecutar_DebeUsarDatosPrelllenadosCuandoExistan()
        {
            // Arrange
            var invitacion = new Invitacion
            {
                Id = 1,
                Email = "socio@test.com",
                Estado = "Pendiente",
                UsuarioId = null,
                GimnasioId = 5,
                CuotaPagadaHasta = DateTime.UtcNow.AddMonths(1),
                DatosPrellenados = JsonSerializer.Serialize(new Dictionary<string, object>
                {
                    { "nombre", "María" },
                    { "apellidos", "González" },
                    { "dni", "87654321" },
                    { "telefono", "987654321" }
                })
            };

            var dto = new RegisterInvitacionDTO
            {
                TokenInvitacion = "simple_token_1",
                NombreUsuario = "mariagonzalez",
                Password = "Password123!",
                FechaNacimiento = new DateTime(1995, 5, 15)
            };

            Socio? socioGuardado = null;
            _mockInvitacionRepo.Setup(r => r.ObtenerPorIdAsync(invitacion.Id)).ReturnsAsync(invitacion);
            _mockInvitacionRepo.Setup(r => r.ActualizarAsync(It.IsAny<Invitacion>())).ReturnsAsync((Invitacion i) => i);
            _mockUsuarioRepo.Setup(r => r.AgregarAsync(It.IsAny<Usuario>()))
                .ReturnsAsync((Usuario u) => { u.Id = 10; socioGuardado = u as Socio; return u; });
            _mockGenerarToken.Setup(g => g.Ejecutar(It.IsAny<Usuario>())).Returns("token");

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            socioGuardado.Should().NotBeNull();
            socioGuardado!.Nombre.Should().Be("María");
            socioGuardado.Apellido.Should().Be("González");
            socioGuardado.Dni.Should().Be(87654321);
            socioGuardado.Telefono.Should().Be("987654321");
            socioGuardado.Email.Should().Be(invitacion.Email);
            socioGuardado.NombreUsuario.Should().Be(dto.NombreUsuario);
            socioGuardado.CuotaPagadaHasta.Should().Be(invitacion.CuotaPagadaHasta);
        }

        [Fact]
        public async Task Ejecutar_DebeCrearSocioConValoresPorDefecto()
        {
            // Arrange
            var invitacion = new Invitacion
            {
                Id = 1,
                Email = "socio@test.com",
                Estado = "Pendiente",
                UsuarioId = null,
                GimnasioId = 5,
                DatosPrellenados = "{}"
            };

            var dto = new RegisterInvitacionDTO
            {
                TokenInvitacion = "simple_token_1",
                NombreUsuario = "socio",
                Password = "Password123!",
                FechaNacimiento = new DateTime(1990, 1, 1)
            };

            Socio? socioGuardado = null;
            _mockInvitacionRepo.Setup(r => r.ObtenerPorIdAsync(invitacion.Id)).ReturnsAsync(invitacion);
            _mockInvitacionRepo.Setup(r => r.ActualizarAsync(It.IsAny<Invitacion>())).ReturnsAsync((Invitacion i) => i);
            _mockUsuarioRepo.Setup(r => r.AgregarAsync(It.IsAny<Usuario>()))
                .Callback<Usuario>(u => { u.Id = 10; socioGuardado = u as Socio; })
                .ReturnsAsync((Usuario u) => u);
            _mockGenerarToken.Setup(g => g.Ejecutar(It.IsAny<Usuario>())).Returns("token");

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            socioGuardado.Should().NotBeNull();
            socioGuardado!.Rol.Should().Be("User");
            socioGuardado.Estado.Should().Be("Activo");
            socioGuardado.EsActivado.Should().BeTrue();
            socioGuardado.Nivel.Should().Be("Inicial");
            socioGuardado.Peso.Should().Be(0);
            socioGuardado.Altura.Should().Be(0);
            socioGuardado.Puntaje.Should().Be(0);
            socioGuardado.FechaRegistro.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task Ejecutar_DebeGenerarTokenYRetornarAuthResponse()
        {
            // Arrange
            var invitacion = new Invitacion
            {
                Id = 1,
                Email = "socio@test.com",
                Estado = "Pendiente",
                UsuarioId = null,
                GimnasioId = 5,
                DatosPrellenados = "{}"
            };

            var dto = new RegisterInvitacionDTO
            {
                TokenInvitacion = "simple_token_1",
                NombreUsuario = "socio",
                Password = "Password123!",
                FechaNacimiento = new DateTime(1990, 1, 1)
            };

            _mockInvitacionRepo.Setup(r => r.ObtenerPorIdAsync(invitacion.Id)).ReturnsAsync(invitacion);
            _mockInvitacionRepo.Setup(r => r.ActualizarAsync(It.IsAny<Invitacion>())).ReturnsAsync((Invitacion i) => i);
            _mockUsuarioRepo.Setup(r => r.AgregarAsync(It.IsAny<Usuario>()))
                .ReturnsAsync((Usuario u) => { u.Id = 20; return u; });
            _mockGenerarToken.Setup(g => g.Ejecutar(It.IsAny<Usuario>())).Returns("jwt_token_generated");

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Token.Should().Be("jwt_token_generated");
            resultado.User.Should().NotBeNull();
            resultado.User.Id.Should().Be(20);
            resultado.User.Email.Should().Be(invitacion.Email);

            _mockGenerarToken.Verify(g => g.Ejecutar(It.IsAny<Usuario>()), Times.Once);
        }
    }
}
