using FitRank_API.Application.CasosDeUso.Ingreso;
using FitRank_API.Application.CasosDeUso.Invitacion;

using FitRank_API.Application.DTOs.IngresoDTOs;
using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using MercadoPago.Client.Payment;
using Newtonsoft.Json.Linq;

namespace FitRank_API.Application.CasosDeUso.MercadoPago
{
    public class ProcesarPagoMercadoPagoCasoDeUso
    {
        private readonly IInvitacionRepositorio _invitacionRepo;
        private readonly IUsuarioRepositorio _usuarioRepo;
        private readonly AgregarIngresoCasoDeUso _agregarIngresoCaso;
        private readonly AgregarInvitacionCasoDeUso _agregarInvitacionCaso;

        public ProcesarPagoMercadoPagoCasoDeUso(
            IInvitacionRepositorio invitacionRepo,
            IUsuarioRepositorio usuarioRepo,
            AgregarIngresoCasoDeUso agregarIngresoCaso,
            AgregarInvitacionCasoDeUso agregarInvitacionCaso)
        {
            _invitacionRepo = invitacionRepo;
            _usuarioRepo = usuarioRepo;
            _agregarIngresoCaso = agregarIngresoCaso;
            _agregarInvitacionCaso = agregarInvitacionCaso;
        }

        public async Task Ejecutar(JObject body)
        {
            var topic = body["type"]?.ToString();
            var dataId = body["data"]?["id"]?.ToString();

            if (topic != "payment" || string.IsNullOrEmpty(dataId))
                return;

            var client = new PaymentClient();
            var payment = await client.GetAsync(long.Parse(dataId));

            if (payment.Status != "approved")
                return;

            // Buscar invitación por email del comprador
            var emailSocio = payment.Payer.Email;
            var invitacion = await _invitacionRepo.ObtenerPorEmailAsync(emailSocio);

            if (invitacion == null)
                return;

            invitacion.Estado = "Pagado";
            invitacion.MetodoPago = "MercadoPago";
            invitacion.MpPaymentId = payment.Id.ToString();
            invitacion.CuotaPagadaHasta = DateTime.UtcNow.AddMonths(1);
            await _invitacionRepo.ActualizarAsync(invitacion);

            // Registrar ingreso
            await _agregarIngresoCaso.Ejecutar(new AgregarIngresoDTO
            {
                GimnasioId = invitacion.GimnasioId,
                UsuarioId = invitacion.UsuarioId,
                Monto = (decimal)payment.TransactionAmount,
                MetodoPago = "MercadoPago",
                Observaciones = $"Pago aprobado - ID {payment.Id}"
            });

            // Reenviar mail con QR usando tu método ya existente
            var socio = await _usuarioRepo.ObtenerPorIdAsync(invitacion.UsuarioId ?? 0);
            if (socio != null)
            {
                var dto = new GenerarInvitacionDTO
                {
                    Nombre = socio.Nombre,
                    Apellidos = socio.Apellido,
                    Email = socio.Email,
                    Telefono = socio.Telefono,
                    MetodoPago = "MercadoPago"
                };
                var token = Guid.NewGuid().ToString("N");

                // Llamamos internamente a tu método de envío QR
                 _agregarInvitacionCaso
      .GetType()
      .GetMethod("ProcesarInvitacionQrAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
      .Invoke(_agregarInvitacionCaso, new object[] { dto, token, invitacion });

            }
        }
    }
}
