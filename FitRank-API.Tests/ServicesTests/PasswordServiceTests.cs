using FitRank_API.Application.Services;
using FluentAssertions;

namespace FitRank_API.Tests.ServicesTests
{
    public class PasswordServiceTests
    {
        private readonly PasswordService _passwordService;

        public PasswordServiceTests()
        {
            _passwordService = new PasswordService();
        }

        [Fact]
        public void HashPassword_DebeGenerarHashDiferenteAlPasswordOriginal()
        {
            // Arrange
            var password = "MiPasswordSegura123!";

            // Act
            var hashedPassword = _passwordService.HashPassword(password);

            // Assert
            hashedPassword.Should().NotBeNullOrEmpty();
            hashedPassword.Should().NotBe(password);
        }

        [Fact]
        public void HashPassword_DebeGenerarHashesDiferentesParaMismaPassword()
        {
            // Arrange
            var password = "Password123!";

            // Act
            var hash1 = _passwordService.HashPassword(password);
            var hash2 = _passwordService.HashPassword(password);

            // Assert
            hash1.Should().NotBe(hash2); // BCrypt genera salt único cada vez
        }

        [Fact]
        public void HashPassword_DebeLanzarExcepcionSiPasswordEsNula()
        {
            // Arrange
            string? password = null;

            // Act & Assert
            var act = () => _passwordService.HashPassword(password!);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("password");
        }

        [Fact]
        public void HashPassword_DebeLanzarExcepcionSiPasswordEsVacia()
        {
            // Arrange
            var password = string.Empty;

            // Act & Assert
            var act = () => _passwordService.HashPassword(password);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("password");
        }

        [Fact]
        public void HashPassword_DebeLanzarExcepcionSiPasswordEsSoloEspacios()
        {
            // Arrange
            var password = "   ";

            // Act & Assert
            var act = () => _passwordService.HashPassword(password);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("password");
        }

        [Fact]
        public void VerifyPassword_DebeRetornarTrueCuandoPasswordCoincide()
        {
            // Arrange
            var password = "PasswordCorrecta123!";
            var hashedPassword = _passwordService.HashPassword(password);

            // Act
            var resultado = _passwordService.VerifyPassword(password, hashedPassword);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_DebeRetornarFalseCuandoPasswordNoCoincide()
        {
            // Arrange
            var passwordCorrecta = "PasswordCorrecta123!";
            var passwordIncorrecta = "PasswordIncorrecta456!";
            var hashedPassword = _passwordService.HashPassword(passwordCorrecta);

            // Act
            var resultado = _passwordService.VerifyPassword(passwordIncorrecta, hashedPassword);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public void VerifyPassword_DebeLanzarExcepcionSiPasswordEsNula()
        {
            // Arrange
            string? password = null;
            var hashedPassword = _passwordService.HashPassword("test");

            // Act & Assert
            var act = () => _passwordService.VerifyPassword(password!, hashedPassword);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("password");
        }

        [Fact]
        public void VerifyPassword_DebeLanzarExcepcionSiHashedPasswordEsNulo()
        {
            // Arrange
            var password = "test";
            string? hashedPassword = null;

            // Act & Assert
            var act = () => _passwordService.VerifyPassword(password, hashedPassword!);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("hashedPassword");
        }

        [Fact]
        public void VerifyPassword_DebeRetornarFalseSiHashEsInvalido()
        {
            // Arrange
            var password = "Password123!";
            var hashInvalido = "hash_invalido_que_no_es_bcrypt";

            // Act
            var resultado = _passwordService.VerifyPassword(password, hashInvalido);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public void HashPassword_DebeGenerarHashConFormatoBCrypt()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var hashedPassword = _passwordService.HashPassword(password);

            // Assert
            hashedPassword.Should().StartWith("$2"); // BCrypt hashes empiezan con $2a$, $2b$, etc.
            hashedPassword.Length.Should().Be(60); // Los hashes BCrypt tienen 60 caracteres
        }

        [Fact]
        public void VerifyPassword_DebeVerificarCorrectamenteMuchasPasswords()
        {
            // Arrange
            var passwords = new[]
            {
                "Password1!",
                "MiClaveSegura123@",
                "Test123#Admin",
                "Usuario2024$Pass",
                "SuperSecret!999"
            };

            // Act & Assert
            foreach (var password in passwords)
            {
                var hash = _passwordService.HashPassword(password);
                var verificacion = _passwordService.VerifyPassword(password, hash);
                
                verificacion.Should().BeTrue($"La password '{password}' debería verificar correctamente");
            }
        }

        [Fact]
        public void VerifyPassword_DebeDistinguirPasswordsSimilares()
        {
            // Arrange
            var password1 = "Password123!";
            var password2 = "Password123"; // Sin el signo de exclamación
            var hash1 = _passwordService.HashPassword(password1);

            // Act
            var verificacionCorrecta = _passwordService.VerifyPassword(password1, hash1);
            var verificacionIncorrecta = _passwordService.VerifyPassword(password2, hash1);

            // Assert
            verificacionCorrecta.Should().BeTrue();
            verificacionIncorrecta.Should().BeFalse();
        }

        [Theory]
        [InlineData("a")]
        [InlineData("AB")]
        [InlineData("123")]
        [InlineData("password")]
        [InlineData("SuperLongPasswordWithMoreThan100CharactersToTestThatTheServiceCanHandleVeryLongPasswordsWithoutAnyIssues")]
        public void HashPassword_DebeAceptarPasswordsDeDiferentesLongitudes(string password)
        {
            // Act
            var hash = _passwordService.HashPassword(password);

            // Assert
            hash.Should().NotBeNullOrEmpty();
            _passwordService.VerifyPassword(password, hash).Should().BeTrue();
        }

        [Fact]
        public void HashPassword_DebeAceptarPasswordsConCaracteresEspeciales()
        {
            // Arrange
            var passwords = new[]
            {
                "Pass@123!",
                "Clave#2024$",
                "Test^&*()_+",
                "Usuario|\\/<>?",
                "Contraseña€¥£"
            };

            // Act & Assert
            foreach (var password in passwords)
            {
                var hash = _passwordService.HashPassword(password);
                hash.Should().NotBeNullOrEmpty();
                _passwordService.VerifyPassword(password, hash).Should().BeTrue();
            }
        }
    }
}
