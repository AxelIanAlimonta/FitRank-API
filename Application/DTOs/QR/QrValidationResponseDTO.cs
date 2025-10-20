using FitRank_API.Application.DTOs.Auth;
using FitRank_API.Application.DTOs.Auth.Invitacion;

namespace FitRank_API.Application.DTOs.QR
{
    public class QrValidationResponseDTO
    {

            public bool Valido { get; set; }
            public string Mensaje { get; set; } = string.Empty;
            public UsuarioAuthDTO? User { get; set; }
            public int? AsistenciaId { get; set; }
        
    }
}
