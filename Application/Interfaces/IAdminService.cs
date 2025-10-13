using FitRank_API.Application.DTOs.Auth;
using FitRank_API.Application.DTOs.Auth.invitacion;
using FitRank_API.Application.DTOs.Qr;
using FitRank_API.Domain.Entities;


namespace FitRank_API.Application.Interfaces
{
    public interface IAdminService
    {
        Task<InvitacionResponseDto> GenerarInvitacionAsync(GenerarInvitacionDto dto, int adminId);
        Task<InvitacionResponseDto> FallbackEfectivoAsync(FallbackEfectivoDto dto, int adminId);
        Task<QrValidationResponseDto> ValidarQrAsync(QrValidationDto dto, int? adminId);
        Task<EmailResponseDto> EnviarEmailQrAsync(EmailDto dto);
        string GenerarQrToken(Invitacion invitacion);
        string GenerarQrImage(string data);
    }
}
