using FitRank_API.Application.DTOs.Auth;
using FitRank_API.Application.DTOs.Auth.Invitacion;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO?> LoginAsync(LoginDTO dto);
        Task<AuthResponseDTO?> RegisterAsync(RegisterDTO dto);
        Task<AuthResponseDTO?> RegisterWithInvitacionAsync(RegisterInvitacionDTO dto);
        string GenerateJwtToken(Usuario user);
        UsuarioAuthDTO MapToUsuarioDto(Usuario user);
        Task<bool> ValidarTokenActivacionAsync(string token);
        Task<string?> ActivarCuentaAsync(string token, string nuevaPassword);
    }
}
