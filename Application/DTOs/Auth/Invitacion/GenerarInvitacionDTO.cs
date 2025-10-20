using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Application.DTOs.Auth.Invitacion
{
    public class GenerarInvitacionDTO
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public int Dni { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        [Required]
        public string MetodoPago { get; set; } = "Efectivo";
        public decimal Monto { get; set; } = 50000m;
        public string Periodo { get; set; } = "Monthly";
    }
}
