using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Application.DTOs.Invitacion
{
    public class RegisterInvitacionDTO
    {
        [Required]
        public string TokenInvitacion { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
       
    }
}
