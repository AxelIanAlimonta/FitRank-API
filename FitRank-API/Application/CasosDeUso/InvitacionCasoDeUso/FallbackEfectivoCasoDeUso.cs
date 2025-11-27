using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Application.Helpers;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FitRank_API.Application.CasosDeUso.Invitacion
{
    public class FallbackEfectivoCasoDeUso
    {
        private readonly IInvitacionRepositorio _invitacionRepositorio;
        private readonly IGimnasioRepositorio _gimnasioRepositorio;
        private readonly IConfiguration _config;
        private readonly QrHelper _qrHelper;

        public FallbackEfectivoCasoDeUso(
            IInvitacionRepositorio invitacionRepositorio,
            IGimnasioRepositorio gimnasioRepositorio,
            IConfiguration config,
            QrHelper qrHelper)
        {
            _invitacionRepositorio = invitacionRepositorio;
            _gimnasioRepositorio = gimnasioRepositorio;
            _config = config;
            _qrHelper = qrHelper;
        }

        public virtual async Task<InvitacionResponseDTO> Ejecutar(FallbackEfectivoDTO dto, int adminId)
        {
           
            var gimnasio = await _gimnasioRepositorio.ObtenerPorAdministradorIdAsync(adminId);
            if (gimnasio == null)
                return new InvitacionResponseDTO
                {
                    Success = false,
                    Mensaje = "No se encontró un gimnasio asociado al administrador."
                };

            var invitacion = await _invitacionRepositorio.ObtenerPorIdYEstadoAsync(dto.InvitacionId, "Pendiente");
            if (invitacion == null)
                return new InvitacionResponseDTO
                {
                    Success = false,
                    Mensaje = "Invitación no encontrada o ya procesada."
                };

            invitacion.Estado = "FallbackEfectivo";
            invitacion.MetodoPago = "Efectivo";
            invitacion.GimnasioId = gimnasio.Id; 
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
                InvitacionId = invitacion.Id,
                QrImage = qrImage,
                TokenInvitacion = tokenInvitacion,
                Mensaje = "Pago confirmado en efectivo. Cuota habilitada por 30 días."
            };
        }

        private async Task<(string tokenInvitacion, string qrImage)> RegenerarQrInvitacion(Domain.Entities.Invitacion invitacion)
        {
           
            var tokenInvitacion = _qrHelper.GenerarQrToken(invitacion);

            var qrData = $"{_config["FrontendUrl"] ?? "http://localhost:4200"}/invitacion?token={tokenInvitacion}";
            var qrImage = await _qrHelper.GenerarQrImage(qrData);

            return (tokenInvitacion, qrImage);
        }
    }
}
