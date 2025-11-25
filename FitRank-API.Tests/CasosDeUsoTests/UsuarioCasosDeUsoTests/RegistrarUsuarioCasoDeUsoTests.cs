using AutoMapper;
using FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.UsuarioCasosDeUsoTests
{
    public class RegistrarUsuarioCasoDeUsoTests
    {
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepo;
        private readonly Mock<GenerarTokenCasoDeUso> _mockGenerarToken;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly IMapper _mapper;
        private readonly RegistrarUsuarioCasoDeUso _casoDeUso;

        public RegistrarUsuarioCasoDeUsoTests()
        {
            _mockUsuarioRepo = new Mock<IUsuarioRepositorio>();
            _mockGenerarToken = new Mock<GenerarTokenCasoDeUso>(MockBehavior.Loose, It.IsAny<IConfiguration>(), It.IsAny<IUsuarioRepositorio>());
            _mockConfig = new Mock<IConfiguration>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegisterDTO, Usuario>();
                cfg.CreateMap<Usuario, UsuarioAuthDTO>();
            });
            _mapper = config.CreateMapper();

            _casoDeUso = new RegistrarUsuarioCasoDeUso(
                _mockUsuarioRepo.Object,
                _mockGenerarToken.Object,
                _mockConfig.Object,
                _mapper
            );
        }

        [Fact]
        public async Task Ejecutar_DebeRegistrarUsuarioCorrectamente()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Email = "test@test.com",
                Password = "Password123!",
                Rol = "User"
            };

            _mockUsuarioRepo.Setup(r => r.ExistePorEmailAsync(dto.Email)).ReturnsAsync(false);
            _mockUsuarioRepo.Setup(r => r.AgregarAsync(It.IsAny<Usuario>())).ReturnsAsync((Usuario u) => u);
            _mockGenerarToken.Setup(g => g.Ejecutar(It.IsAny<Usuario>())).Returns("fake_token");

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Token.Should().Be("fake_token");
            resultado.User.Should().NotBeNull();
            resultado.User.Email.Should().Be(dto.Email);

            _mockUsuarioRepo.Verify(r => r.AgregarAsync(It.Is<Usuario>(u =>
                u.Email == dto.Email &&
                u.Rol == "User" &&
                u.Estado == "Activo" &&
                !string.IsNullOrEmpty(u.PasswordHash)
            )), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarNullSiEmailYaExiste()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Email = "existe@test.com",
                Password = "Password123!"
            };

            _mockUsuarioRepo.Setup(r => r.ExistePorEmailAsync(dto.Email)).ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().BeNull();
            _mockUsuarioRepo.Verify(r => r.AgregarAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task Ejecutar_DebeHashearPasswordCorrectamente()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Email = "test@test.com",
                Password = "Password123!",
                Rol = "User"
            };

            Usuario? usuarioGuardado = null;
            _mockUsuarioRepo.Setup(r => r.ExistePorEmailAsync(dto.Email)).ReturnsAsync(false);
            _mockUsuarioRepo.Setup(r => r.AgregarAsync(It.IsAny<Usuario>()))
                .Callback<Usuario>(u => usuarioGuardado = u)
                .ReturnsAsync((Usuario u) => u);
            _mockGenerarToken.Setup(g => g.Ejecutar(It.IsAny<Usuario>())).Returns("token");

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            usuarioGuardado.Should().NotBeNull();
            usuarioGuardado!.PasswordHash.Should().NotBe(dto.Password);
            BCrypt.Net.BCrypt.Verify(dto.Password, usuarioGuardado.PasswordHash).Should().BeTrue();
        }

        [Fact]
        public async Task Ejecutar_DebeAsignarRolUserPorDefectoSiNoSeProvee()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Email = "test@test.com",
                Password = "Password123!",
                Rol = string.Empty
            };

            Usuario? usuarioGuardado = null;
            _mockUsuarioRepo.Setup(r => r.ExistePorEmailAsync(dto.Email)).ReturnsAsync(false);
            _mockUsuarioRepo.Setup(r => r.AgregarAsync(It.IsAny<Usuario>()))
                .Callback<Usuario>(u => usuarioGuardado = u)
                .ReturnsAsync((Usuario u) => u);
            _mockGenerarToken.Setup(g => g.Ejecutar(It.IsAny<Usuario>())).Returns("token");

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            usuarioGuardado.Should().NotBeNull();
            usuarioGuardado!.Rol.Should().Be("User");
        }

        [Fact]
        public async Task Ejecutar_DebeAsignarEstadoActivoPorDefecto()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Email = "test@test.com",
                Password = "Password123!",
                Rol = "User"
            };

            Usuario? usuarioGuardado = null;
            _mockUsuarioRepo.Setup(r => r.ExistePorEmailAsync(dto.Email)).ReturnsAsync(false);
            _mockUsuarioRepo.Setup(r => r.AgregarAsync(It.IsAny<Usuario>()))
                .Callback<Usuario>(u => usuarioGuardado = u)
                .ReturnsAsync((Usuario u) => u);
            _mockGenerarToken.Setup(g => g.Ejecutar(It.IsAny<Usuario>())).Returns("token");

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            usuarioGuardado.Should().NotBeNull();
            usuarioGuardado!.Estado.Should().Be("Activo");
        }

        [Fact]
        public async Task Ejecutar_DebeGenerarTokenDespuesDeRegistrar()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Email = "test@test.com",
                Password = "Password123!",
                Rol = "Admin"
            };

            _mockUsuarioRepo.Setup(r => r.ExistePorEmailAsync(dto.Email)).ReturnsAsync(false);
            _mockUsuarioRepo.Setup(r => r.AgregarAsync(It.IsAny<Usuario>())).ReturnsAsync((Usuario u) => u);
            _mockGenerarToken.Setup(g => g.Ejecutar(It.IsAny<Usuario>())).Returns("generated_token_12345");

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Token.Should().Be("generated_token_12345");
            _mockGenerarToken.Verify(g => g.Ejecutar(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarAgregarAsyncConUsuarioMapeado()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Email = "nuevo@test.com",
                Password = "Password123!",
                Rol = "User"
            };

            _mockUsuarioRepo.Setup(r => r.ExistePorEmailAsync(dto.Email)).ReturnsAsync(false);
            _mockUsuarioRepo.Setup(r => r.AgregarAsync(It.IsAny<Usuario>())).ReturnsAsync((Usuario u) => u);
            _mockGenerarToken.Setup(g => g.Ejecutar(It.IsAny<Usuario>())).Returns("token_generado");

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            _mockUsuarioRepo.Verify(r => r.AgregarAsync(It.Is<Usuario>(u => 
                u.Email == dto.Email
            )), Times.Once);
        }
    }
}
