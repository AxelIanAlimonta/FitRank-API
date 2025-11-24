using FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace FitRank_API.Tests.CasosDeUsoTests.UsuarioCasosDeUsoTests
{
    public class ActivarCuentaCasoDeUsoTests
    {
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepo;
        private readonly ActivarCuentaCasoDeUso _casoDeUso;

        public ActivarCuentaCasoDeUsoTests()
        {
            _mockUsuarioRepo = new Mock<IUsuarioRepositorio>();
            _casoDeUso = new ActivarCuentaCasoDeUso(_mockUsuarioRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeActivarCuentaCorrectamente()
        {
            // Arrange
            var token = "token_valido_123";
            var nuevaPassword = "NuevaPassword123!";

            var usuario = new Usuario
            {
                Id = 1,
                Email = "usuario@test.com",
                PasswordHash = "hash_antiguo",
                TokenRecuperacion = token,
                TokenExpira = DateTime.UtcNow.AddHours(1),
                EsActivado = false,
                NombreUsuario = string.Empty
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync(usuario);
            _mockUsuarioRepo.Setup(r => r.ActualizarAsync(It.IsAny<Usuario>())).ReturnsAsync((Usuario u) => u);

            // Act
            var resultado = await _casoDeUso.Ejecutar(token, nuevaPassword);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().Be(usuario.Email);

            _mockUsuarioRepo.Verify(r => r.ActualizarAsync(It.Is<Usuario>(u =>
                u.EsActivado == true &&
                u.TokenRecuperacion == null &&
                u.TokenExpira == null
            )), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarNullSiTokenInvalido()
        {
            // Arrange
            var token = "token_invalido";
            var nuevaPassword = "Password123!";

            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync((Usuario?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(token, nuevaPassword);

            // Assert
            resultado.Should().BeNull();
            _mockUsuarioRepo.Verify(r => r.ActualizarAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarNullSiTokenExpirado()
        {
            // Arrange
            var token = "token_expirado";
            var nuevaPassword = "Password123!";

            var usuario = new Usuario
            {
                Id = 1,
                Email = "usuario@test.com",
                TokenRecuperacion = token,
                TokenExpira = DateTime.UtcNow.AddHours(-1), // Expirado
                EsActivado = false
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync((Usuario?)null); // No encuentra porque está expirado

            // Act
            var resultado = await _casoDeUso.Ejecutar(token, nuevaPassword);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task Ejecutar_DebeHashearNuevaPassword()
        {
            // Arrange
            var token = "token_valido";
            var nuevaPassword = "MiNuevaPassword123!";

            var usuario = new Usuario
            {
                Id = 1,
                Email = "usuario@test.com",
                PasswordHash = "hash_antiguo",
                TokenRecuperacion = token,
                TokenExpira = DateTime.UtcNow.AddHours(1),
                EsActivado = false,
                NombreUsuario = "usuario"
            };

            Usuario? usuarioActualizado = null;
            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync(usuario);
            _mockUsuarioRepo.Setup(r => r.ActualizarAsync(It.IsAny<Usuario>()))
                .Callback<Usuario>(u => usuarioActualizado = u)
                .ReturnsAsync((Usuario u) => u);

            // Act
            await _casoDeUso.Ejecutar(token, nuevaPassword);

            // Assert
            usuarioActualizado.Should().NotBeNull();
            usuarioActualizado!.PasswordHash.Should().NotBe("hash_antiguo");
            usuarioActualizado.PasswordHash.Should().NotBe(nuevaPassword);
            BCrypt.Net.BCrypt.Verify(nuevaPassword, usuarioActualizado.PasswordHash).Should().BeTrue();
        }

        [Fact]
        public async Task Ejecutar_DebeLimpiarTokenRecuperacionYExpiracion()
        {
            // Arrange
            var token = "token_valido";
            var nuevaPassword = "Password123!";

            var usuario = new Usuario
            {
                Id = 1,
                Email = "usuario@test.com",
                PasswordHash = "hash",
                TokenRecuperacion = token,
                TokenExpira = DateTime.UtcNow.AddHours(1),
                EsActivado = false,
                NombreUsuario = "usuario"
            };

            Usuario? usuarioActualizado = null;
            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync(usuario);
            _mockUsuarioRepo.Setup(r => r.ActualizarAsync(It.IsAny<Usuario>()))
                .Callback<Usuario>(u => usuarioActualizado = u)
                .ReturnsAsync((Usuario u) => u);

            // Act
            await _casoDeUso.Ejecutar(token, nuevaPassword);

            // Assert
            usuarioActualizado.Should().NotBeNull();
            usuarioActualizado!.TokenRecuperacion.Should().BeNull();
            usuarioActualizado.TokenExpira.Should().BeNull();
        }

        [Fact]
        public async Task Ejecutar_DebeMarcarComoActivado()
        {
            // Arrange
            var token = "token_valido";
            var nuevaPassword = "Password123!";

            var usuario = new Usuario
            {
                Id = 1,
                Email = "usuario@test.com",
                PasswordHash = "hash",
                TokenRecuperacion = token,
                TokenExpira = DateTime.UtcNow.AddHours(1),
                EsActivado = false,
                NombreUsuario = "usuario"
            };

            Usuario? usuarioActualizado = null;
            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync(usuario);
            _mockUsuarioRepo.Setup(r => r.ActualizarAsync(It.IsAny<Usuario>()))
                .Callback<Usuario>(u => usuarioActualizado = u)
                .ReturnsAsync((Usuario u) => u);

            // Act
            await _casoDeUso.Ejecutar(token, nuevaPassword);

            // Assert
            usuarioActualizado.Should().NotBeNull();
            usuarioActualizado!.EsActivado.Should().BeTrue();
        }

        [Fact]
        public async Task Ejecutar_DebeAsignarNombreUsuarioSiEsNulo()
        {
            // Arrange
            var token = "token_valido";
            var nuevaPassword = "Password123!";

            var usuario = new Usuario
            {
                Id = 1,
                Email = "nuevousuario@test.com",
                PasswordHash = "hash",
                TokenRecuperacion = token,
                TokenExpira = DateTime.UtcNow.AddHours(1),
                EsActivado = false,
                NombreUsuario = string.Empty // Sin nombre de usuario
            };

            Usuario? usuarioActualizado = null;
            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync(usuario);
            _mockUsuarioRepo.Setup(r => r.ActualizarAsync(It.IsAny<Usuario>()))
                .Callback<Usuario>(u => usuarioActualizado = u)
                .ReturnsAsync((Usuario u) => u);

            // Act
            await _casoDeUso.Ejecutar(token, nuevaPassword);

            // Assert
            usuarioActualizado.Should().NotBeNull();
            usuarioActualizado!.NombreUsuario.Should().Be("nuevousuario");
        }

        [Fact]
        public async Task Ejecutar_NoDebeModificarNombreUsuarioSiYaExiste()
        {
            // Arrange
            var token = "token_valido";
            var nuevaPassword = "Password123!";

            var usuario = new Usuario
            {
                Id = 1,
                Email = "usuario@test.com",
                PasswordHash = "hash",
                TokenRecuperacion = token,
                TokenExpira = DateTime.UtcNow.AddHours(1),
                EsActivado = false,
                NombreUsuario = "mi_usuario_existente"
            };

            Usuario? usuarioActualizado = null;
            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync(usuario);
            _mockUsuarioRepo.Setup(r => r.ActualizarAsync(It.IsAny<Usuario>()))
                .Callback<Usuario>(u => usuarioActualizado = u)
                .ReturnsAsync((Usuario u) => u);

            // Act
            await _casoDeUso.Ejecutar(token, nuevaPassword);

            // Assert
            usuarioActualizado.Should().NotBeNull();
            usuarioActualizado!.NombreUsuario.Should().Be("mi_usuario_existente");
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarEmailDelUsuario()
        {
            // Arrange
            var token = "token_valido";
            var nuevaPassword = "Password123!";

            var usuario = new Usuario
            {
                Id = 1,
                Email = "activado@test.com",
                PasswordHash = "hash",
                TokenRecuperacion = token,
                TokenExpira = DateTime.UtcNow.AddHours(1),
                EsActivado = false,
                NombreUsuario = "usuario"
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorCondicionAsync(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .ReturnsAsync(usuario);
            _mockUsuarioRepo.Setup(r => r.ActualizarAsync(It.IsAny<Usuario>())).ReturnsAsync((Usuario u) => u);

            // Act
            var resultado = await _casoDeUso.Ejecutar(token, nuevaPassword);

            // Assert
            resultado.Should().Be("activado@test.com");
        }
    }
}
