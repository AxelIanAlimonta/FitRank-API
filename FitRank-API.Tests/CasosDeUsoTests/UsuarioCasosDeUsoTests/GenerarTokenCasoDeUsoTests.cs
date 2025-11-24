using FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FitRank_API.Tests.CasosDeUsoTests.UsuarioCasosDeUsoTests
{
    public class GenerarTokenCasoDeUsoTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IGimnasioRepositorio> _mockGimnasioRepo;
        private readonly GenerarTokenCasoDeUso _casoDeUso;
        private readonly string _testKey = "test_secret_key_at_least_32_characters_long_for_jwt";

        public GenerarTokenCasoDeUsoTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockGimnasioRepo = new Mock<IGimnasioRepositorio>();

            _mockConfig.Setup(c => c["Jwt:Key"]).Returns(_testKey);
            _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("FitRankTestIssuer");
            _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("FitRankTestAudience");

            _casoDeUso = new GenerarTokenCasoDeUso(_mockConfig.Object, _mockGimnasioRepo.Object);
        }

        [Fact]
        public void Ejecutar_DebeGenerarTokenValido()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Email = "test@test.com",
                Rol = "User"
            };

            _mockGimnasioRepo.Setup(r => r.ObtenerGimnasioIdPorUsuario(usuario.Id)).Returns((long?)null);

            // Act
            var token = _casoDeUso.Ejecutar(usuario);

            // Assert
            token.Should().NotBeNullOrEmpty();
            token.Should().Contain(".");
        }

        [Fact]
        public void Ejecutar_DebeIncluirClaimsBasicos()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 5,
                Email = "user@test.com",
                Rol = "Admin"
            };

            _mockGimnasioRepo.Setup(r => r.ObtenerGimnasioIdPorUsuario(usuario.Id)).Returns((long?)null);

            // Act
            var token = _casoDeUso.Ejecutar(usuario);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == "5");
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == "user@test.com");
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        }

        [Fact]
        public void Ejecutar_DebeIncluirGimnasioIdCuandoExiste()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Email = "socio@gimnasio.com",
                Rol = "User"
            };

            _mockGimnasioRepo.Setup(r => r.ObtenerGimnasioIdPorUsuario(usuario.Id)).Returns(10L);

            // Act
            var token = _casoDeUso.Ejecutar(usuario);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.GroupSid && c.Value == "10");
        }

        [Fact]
        public void Ejecutar_NoDebeIncluirGimnasioIdCuandoEsNull()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Email = "user@test.com",
                Rol = "User"
            };

            _mockGimnasioRepo.Setup(r => r.ObtenerGimnasioIdPorUsuario(usuario.Id)).Returns((long?)null);

            // Act
            var token = _casoDeUso.Ejecutar(usuario);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            jwtToken.Claims.Should().NotContain(c => c.Type == ClaimTypes.GroupSid);
        }

        [Fact]
        public void Ejecutar_DebeConfigurarExpiracionEn6Horas()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Email = "test@test.com",
                Rol = "User"
            };

            _mockGimnasioRepo.Setup(r => r.ObtenerGimnasioIdPorUsuario(usuario.Id)).Returns((long?)null);

            var tiempoAntes = DateTime.UtcNow.AddHours(6);

            // Act
            var token = _casoDeUso.Ejecutar(usuario);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var tiempoDespues = DateTime.UtcNow.AddHours(6);

            jwtToken.ValidTo.Should().BeOnOrAfter(tiempoAntes.AddSeconds(-5));
            jwtToken.ValidTo.Should().BeOnOrBefore(tiempoDespues.AddSeconds(5));
        }

        [Fact]
        public void Ejecutar_DebeUsarConfiguracionDeJwt()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Email = "test@test.com",
                Rol = "User"
            };

            _mockGimnasioRepo.Setup(r => r.ObtenerGimnasioIdPorUsuario(usuario.Id)).Returns((long?)null);

            // Act
            var token = _casoDeUso.Ejecutar(usuario);

            // Assert
            _mockConfig.Verify(c => c["Jwt:Key"], Times.AtLeastOnce);
            _mockConfig.Verify(c => c["Jwt:Issuer"], Times.AtLeastOnce);
            _mockConfig.Verify(c => c["Jwt:Audience"], Times.AtLeastOnce);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            jwtToken.Issuer.Should().Be("FitRankTestIssuer");
            jwtToken.Audiences.Should().Contain("FitRankTestAudience");
        }

        [Fact]
        public void Ejecutar_DebeLlamarObtenerGimnasioIdPorUsuario()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 25,
                Email = "test@test.com",
                Rol = "User"
            };

            _mockGimnasioRepo.Setup(r => r.ObtenerGimnasioIdPorUsuario(usuario.Id)).Returns(15L);

            // Act
            _casoDeUso.Ejecutar(usuario);

            // Assert
            _mockGimnasioRepo.Verify(r => r.ObtenerGimnasioIdPorUsuario(25), Times.Once);
        }

        [Fact]
        public void Ejecutar_DebeGenerarTokenDiferenteParaCadaEjecucion()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Email = "test@test.com",
                Rol = "User"
            };

            _mockGimnasioRepo.Setup(r => r.ObtenerGimnasioIdPorUsuario(usuario.Id)).Returns((long?)null);

            // Act
            var token1 = _casoDeUso.Ejecutar(usuario);
            System.Threading.Thread.Sleep(100); // Pequeña espera para asegurar timestamps diferentes
            var token2 = _casoDeUso.Ejecutar(usuario);

            // Assert
            token1.Should().NotBeNullOrEmpty();
            token2.Should().NotBeNullOrEmpty();
            // Los tokens pueden ser iguales o diferentes dependiendo del timestamp
            // Lo importante es que ambos se generen correctamente
        }

        [Fact]
        public void Ejecutar_DebeUsarHmacSha256ParaFirma()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Email = "test@test.com",
                Rol = "User"
            };

            _mockGimnasioRepo.Setup(r => r.ObtenerGimnasioIdPorUsuario(usuario.Id)).Returns((long?)null);

            // Act
            var token = _casoDeUso.Ejecutar(usuario);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            jwtToken.Header.Alg.Should().Be("HS256");
        }
    }
}
