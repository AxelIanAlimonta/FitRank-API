using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Application.DTOs.Auth.invitacion
{
    public class RegisterInvitacionDto
    {
        [Required]
        public string TokenInvitacion { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public int AlturaCm { get; set; }
        public double PesoKg { get; set; }
        public string Nivel { get; set; } = "Principiante";
    }
}
