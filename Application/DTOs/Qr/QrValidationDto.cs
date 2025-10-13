using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Application.DTOs.Qr
{
    public class QrValidationDto
    {
        [Required]
        public string QrData { get; set; } = string.Empty;
        public int? GymId { get; set; }
        public string? Observaciones { get; set; }
    }
}
