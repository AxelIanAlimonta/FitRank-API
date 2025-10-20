using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Application.DTOs.Auth
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
        public string Correo { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        public int AlturaCm { get; set; }
        public double PesoKg { get; set; }
        public string Nivel { get; set; } = "Principiante";
        public string Rol { get; set; } = "User";
    }
}
