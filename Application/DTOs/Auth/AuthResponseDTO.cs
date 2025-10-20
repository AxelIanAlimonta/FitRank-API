using FitRank_API.Application.DTOs.Auth.Invitacion;

namespace FitRank_API.Application.DTOs.Auth
{
    public class AuthResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public UsuarioAuthDTO User { get; set; } = null!;
    }
}
