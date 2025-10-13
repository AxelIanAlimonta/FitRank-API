using FitRank_API.Application.DTOs.Auth.invitacion;
using FitRank_API.Application.DTOs.Auth;
using FitRank_API.Application.DTOs.Usuario;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> RegisterWithInvitacionAsync(RegisterInvitacionDto dto);
        string GenerateJwtToken(Usuario user);
        UsuarioAuthDto MapToUsuarioDto(Usuario user);
        Task<bool> ValidarTokenActivacionAsync(string token);
        Task<string?> ActivarCuentaAsync(string token, string nuevaPassword);

    }
}
