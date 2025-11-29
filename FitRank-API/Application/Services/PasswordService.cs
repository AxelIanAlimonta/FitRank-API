using FitRank_API.Application.Interfaces;

namespace FitRank_API.Application.Services
{

    public class PasswordService : IPasswordService
    {
      
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password), "La contraseña no puede ser nula o vacía");
            }

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

       
        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password), "La contraseña no puede ser nula o vacía");
            }

            if (string.IsNullOrWhiteSpace(hashedPassword))
            {
                throw new ArgumentNullException(nameof(hashedPassword), "El hash de la contraseña no puede ser nulo o vacío");
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return false;
            }
        }
    }
}
