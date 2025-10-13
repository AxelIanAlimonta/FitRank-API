using FitRank_API.Application.DTOs.Usuario;

namespace FitRank_API.Application.DTOs.Auth
{
    public class AuthResponseDto
    {

        public string Token { get; set; } = string.Empty;
        public UsuarioAuthDto User { get; set; } = null!;
    }
}
