using FitRank_API.Application.CasosDeUso.Ingreso;
using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.DTOs.IngresoDTOs;
using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Infrastructure.Interfaces;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using MercadoPago.Resource.Payment;

namespace FitRank_API.Application.CasosDeUso.MercadoPago
{
    public class ProcesarWebhookPagoCasoDeUso
    {
        private readonly IInvitacionRepositorio _invitacionRepo;
        private readonly IUsuarioRepositorio _usuarioRepo;
        private readonly AgregarIngresoCasoDeUso _agregarIngresoCaso;
        private readonly AgregarInvitacionCasoDeUso _agregarInvitacionCaso;
        private readonly IConfiguration _config;

        public ProcesarWebhookPagoCasoDeUso(
            IInvitacionRepositorio invitacionRepo,
            IUsuarioRepositorio usuarioRepo,
            AgregarIngresoCasoDeUso agregarIngresoCaso,
            AgregarInvitacionCasoDeUso agregarInvitacionCaso,
            IConfiguration config)
        {
            _invitacionRepo = invitacionRepo;
            _usuarioRepo = usuarioRepo;
            _agregarIngresoCaso = agregarIngresoCaso;
            _agregarInvitacionCaso = agregarInvitacionCaso;
            _config = config;
        }

        // ⚠️ Ahora recibe un long, NO el body
        public async Task Ejecutar(long paymentId)
        {
            try
            {
                MercadoPagoConfig.AccessToken = _config["MercadoPago:AccessToken"];

                var client = new PaymentClient();
                Payment payment = await client.GetAsync(paymentId);

                if (payment == null)
                {
                    Console.WriteLine("⚠ Pago no encontrado en MP.");
                    return;
                }

                if (payment.Status != "approved")
                {
                    Console.WriteLine("⚠ Pago recibido pero no aprobado.");
                    return;
                }

                // 🎯 Recuperamos la invitación asociada
                long invitacionId = long.Parse(payment.ExternalReference);
                var invitacion = await _invitacionRepo.ObtenerPorIdAsync(invitacionId);

                if (invitacion == null)
                {
                    Console.WriteLine($"❌ Invitación {invitacionId} no encontrada.");
                    return;
                }

                var socio = await _usuarioRepo.ObtenerPorIdAsync(invitacion.UsuarioId ?? 0);
                if (socio == null)
                {
                    Console.WriteLine("❌ Socio no encontrado.");
                    return;
                }

                // 💰 Registrar ingreso
                await _agregarIngresoCaso.Ejecutar(new AgregarIngresoDTO
                {
                    GimnasioId = invitacion.GimnasioId,
                    UsuarioId = socio.Id,
                    MetodoPago = "MercadoPago",
                    Monto = (decimal)payment.TransactionAmount,
                    Observaciones = "Pago acreditado por Mercado Pago"
                });

                // 📩 Reenviar QR y activación
                await _agregarInvitacionCaso.ProcesarInvitacionQrAsync(
                    new GenerarInvitacionDTO
                    {
                        Nombre = socio.Nombre,
                        Apellidos = socio.Apellido,
                        Email = socio.Email,
                        Telefono = socio.Telefono
                    },
                    socio.TokenRecuperacion,
                    invitacion
                );

                // ✔ Actualizar estado
                invitacion.Estado = "Pagado";
                invitacion.MpPaymentId = paymentId.ToString();
                await _invitacionRepo.ActualizarAsync(invitacion);

                Console.WriteLine("💚 PAGO PROCESADO OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error procesando pago MP: " + ex);
            }
        }
    }
}
