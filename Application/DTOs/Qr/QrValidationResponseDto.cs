using FitRank_API.Application.DTOs.Usuario;
using FitRank_API.Application.DTOs.Auth;
            

namespace FitRank_API.Application.DTOs.Qr
{
    public class QrValidationResponseDto
    {
        public bool Valido { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public UsuarioAuthDto? User { get; set; }
        public int? AsistenciaId { get; set; }
    }
}
