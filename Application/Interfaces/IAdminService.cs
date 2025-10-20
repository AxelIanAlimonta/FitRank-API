using FitRank_API.Application.DTOs.Auth;
using FitRank_API.Application.DTOs.Auth.Invitacion;
using FitRank_API.Application.DTOs.QR;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Interfaces
{
    public interface IAdminService
    {
        Task<InvitacionResponseDTO> GenerarInvitacionAsync(GenerarInvitacionDTO dto, int adminId);
        Task<InvitacionResponseDTO> FallbackEfectivoAsync(FallbackEfectivoDTO dto, int adminId);
        Task<QrValidationResponseDTO> ValidarQrAsync(QrValidationDTO dto, int? adminId);
        Task<EmailResponseDTO> EnviarEmailQrAsync(EmailDTO dto);
        string GenerarQrToken(Invitacion invitacion);
        string GenerarQrImage(string data);
    }
}
