using AutoMapper;
using FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace FitRank_API.Tests.CasosDeUsoTests.UsuarioCasosDeUsoTests
{
    public class LoginUsuarioCasoDeUsoTests
    {
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepo;
        private readonly Mock<IPasswordService> _mockPasswordService;
        private readonly IMapper _mapper;
        private readonly LoginUsuarioCasoDeUso _casoDeUso;

        public LoginUsuarioCasoDeUsoTests()
        {
            _mockUsuarioRepo = new Mock<IUsuarioRepositorio>();
            _mockPasswordService = new Mock<IPasswordService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Usuario, UsuarioAuthDTO>();
            });
            _mapper = config.CreateMapper();

            _casoDeUso = new LoginUsuarioCasoDeUso(_mockUsuarioRepo.Object, _mapper, _mockPasswordService.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeLoginCorrectamenteConCredencialesValidas()
        {
            // Arrange
            var password = "Password123!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var usuario = new Usuario
            {
                Id = 1,
                Email = "user@test.com",
                PasswordHash = hashedPassword,
                EsActivado = true,
                Rol = "User",
                Estado = "Activo"
            };

            var dto = new LoginDTO
            {
                Email = "user@test.com",
                Password = password
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync(usuario);
            _mockPasswordService.Setup(p => p.VerifyPassword(password, hashedPassword)).Returns(true);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Value.entidad.Should().NotBeNull();
            resultado.Value.entidad.Email.Should().Be(usuario.Email);
            resultado.Value.dto.Should().NotBeNull();
            resultado.Value.dto.Email.Should().Be(usuario.Email);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarNullSiEmailNoExiste()
        {
            // Arrange
            var dto = new LoginDTO
            {
                Email = "noexiste@test.com",
                Password = "Password123!"
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync((Usuario?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarNullSiPasswordEsIncorrecta()
        {
            // Arrange
            var correctPassword = "Password123!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(correctPassword);

            var usuario = new Usuario
            {
                Id = 1,
                Email = "user@test.com",
                PasswordHash = hashedPassword,
                EsActivado = true
            };

            var dto = new LoginDTO
            {
                Email = "user@test.com",
                Password = "PasswordIncorrecta!"
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync(usuario);
            _mockPasswordService.Setup(p => p.VerifyPassword("PasswordIncorrecta!", hashedPassword)).Returns(false);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarNullSiUsuarioNoEstaActivado()
        {
            // Arrange
            var password = "Password123!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var usuario = new Usuario
            {
                Id = 1,
                Email = "user@test.com",
                PasswordHash = hashedPassword,
                EsActivado = false // Usuario no activado
            };

            var dto = new LoginDTO
            {
                Email = "user@test.com",
                Password = password
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync(usuario);
            _mockPasswordService.Setup(p => p.VerifyPassword(password, hashedPassword)).Returns(true);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task Ejecutar_DebeValidarPasswordConBCrypt()
        {
            // Arrange
            var password = "MiPasswordSegura123!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var usuario = new Usuario
            {
                Id = 1,
                Email = "user@test.com",
                PasswordHash = hashedPassword,
                EsActivado = true
            };

            var dto = new LoginDTO
            {
                Email = "user@test.com",
                Password = password
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync(usuario);
            _mockPasswordService.Setup(p => p.VerifyPassword(password, hashedPassword)).Returns(true);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            _mockPasswordService.Verify(p => p.VerifyPassword(password, hashedPassword), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeBuscarUsuarioPorEmail()
        {
            // Arrange
            var dto = new LoginDTO
            {
                Email = "buscar@test.com",
                Password = "Password123!"
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync((Usuario?)null);

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            _mockUsuarioRepo.Verify(r => r.ObtenerPorCondicionAsync(
                It.IsAny<Expression<Func<Usuario, bool>>>()), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarEntidadYDtoCorrectamente()
        {
            // Arrange
            var password = "Password123!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var usuario = new Usuario
            {
                Id = 5,
                Email = "socio@test.com",
                PasswordHash = hashedPassword,
                EsActivado = true,
                Rol = "Admin",
                Estado = "Activo"
            };

            var dto = new LoginDTO
            {
                Email = "socio@test.com",
                Password = password
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync(usuario);
            _mockPasswordService.Setup(p => p.VerifyPassword(password, hashedPassword)).Returns(true);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Value.entidad.Should().Be(usuario);
            resultado.Value.dto.Should().NotBeNull();
            resultado.Value.dto.Id.Should().Be(5);
            resultado.Value.dto.Email.Should().Be("socio@test.com");
        }
    }
}
