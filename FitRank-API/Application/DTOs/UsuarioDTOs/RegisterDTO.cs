using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Application.DTOs.UsuarioDTOs
{
    public class RegisterDTO
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public int Dni { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Telefono { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        
        public string Rol { get; set; } = "User";
    }
}
