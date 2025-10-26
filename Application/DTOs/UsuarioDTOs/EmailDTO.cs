using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Application.DTOs.UsuarioDTOs
{
    public class EmailDTO
    {
        public int UsuarioId { get; set; }
        [Required]
        [EmailAddress]
        public string EmailDestinatario { get; set; } = string.Empty;
    }
}
