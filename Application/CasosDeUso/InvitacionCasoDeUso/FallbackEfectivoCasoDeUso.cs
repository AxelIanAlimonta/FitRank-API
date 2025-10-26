using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Application.Helpers;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FitRank_API.Application.CasosDeUso.Invitacion
{
    public class FallbackEfectivoCasoDeUso
    {
        private readonly IInvitacionRepositorio _invitacionRepositorio;
        private readonly IConfiguration _config;
        private readonly QrHelper _qrHelper;

        public FallbackEfectivoCasoDeUso(
            IInvitacionRepositorio invitacionRepositorio,
            IConfiguration config,
            QrHelper qrHelper)
        {
            _invitacionRepositorio = invitacionRepositorio;
            _config = config;
            _qrHelper = qrHelper;
        }

        public async Task<InvitacionResponseDTO> Ejecutar(FallbackEfectivoDTO dto, int adminId)
        {
            var invitacion = await _invitacionRepositorio.ObtenerPorIdYEstadoAsync(dto.InvitacionId, "Pendiente");
            if (invitacion == null)
                return new InvitacionResponseDTO { Success = false, Mensaje = "Invitación no encontrada" };


            invitacion.Estado = "FallbackEfectivo";
            invitacion.MetodoPago = "Efectivo";
            invitacion.CuotaPagadaHasta = DateTime.Now.AddDays(30);

            (string tokenInvitacion, string qrImage) = await RegenerarQrInvitacion(invitacion);

            await _invitacionRepositorio.ActualizarAsync(invitacion);

            return CrearInvitacionResponse(invitacion, tokenInvitacion, qrImage);
        }

        private static InvitacionResponseDTO CrearInvitacionResponse(Domain.Entities.Invitacion invitacion, string tokenInvitacion, string qrImage)
        {
            return new InvitacionResponseDTO
            {
                Success = true,
                QrImage = qrImage,
                TokenInvitacion = tokenInvitacion,
                Mensaje = "Fallback a efectivo confirmado. Cuota pagada hasta 30 días.",
                InvitacionId = invitacion.Id
            };
        }

        private async Task<(string tokenInvitacion, string qrImage)> RegenerarQrInvitacion(Domain.Entities.Invitacion invitacion)
        {
            // Regenera QR
            var tokenInvitacion = _qrHelper.GenerarQrToken(invitacion);
            var qrData = $"{_config["FrontendUrl"] ?? "http://localhost:4200"}/invitacion?token={tokenInvitacion}";
            var qrImage = await _qrHelper.GenerarQrImage(qrData);
            return (tokenInvitacion, qrImage);
        }
    }
}
