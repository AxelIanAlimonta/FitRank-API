namespace FitRank_API.Application.Interfaces
{
    /// <summary>
    /// Servicio para encriptar y verificar contraseñas
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// Encripta una contraseña en texto plano
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Hash de la contraseña</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifica si una contraseña coincide con su hash
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <param name="hashedPassword">Hash almacenado</param>
        /// <returns>True si coinciden, False si no</returns>
        bool VerifyPassword(string password, string hashedPassword);
    }
}
