using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Application.DTOs.QR
{
    public class QrValidationDTO
    {

            [Required]
            public string QrData { get; set; } = string.Empty;
            public int? GymId { get; set; }
            public string? Observaciones { get; set; }
        
    }
}
