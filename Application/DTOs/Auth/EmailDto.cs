using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Application.DTOs.Auth
{
    public class EmailDto
    {
        public int UsuarioId { get; set; }
        [Required]
        [EmailAddress]
        public string EmailDestinatario { get; set; } = string.Empty;
    }
}
