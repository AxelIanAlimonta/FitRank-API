using FitRank_API.Application.DTOs.UsuarioDTOs;
namespace FitRank_API.Application.DTOs.UsuarioDTOs
{
    public class AuthResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public UsuarioAuthDTO User { get; set; } = null!;
    }
}
