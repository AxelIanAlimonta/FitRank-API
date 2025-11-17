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

        public async Task Ejecutar(dynamic body)
        {
            try
            {
                // MP envía múltiples notificaciones. Esta es la que importa.
                if ((string)body?.type != "payment")
                    return;

                long paymentId = long.Parse(body.data.id.ToString());

                // SDK MP
                MercadoPagoConfig.AccessToken = _config["MercadoPago:AccessToken"];
                var client = new PaymentClient();
                Payment payment = await client.GetAsync(paymentId);

                if (payment.Status != "approved")
                    return;

                // ⭐ InvitacionId que pusiste en ExternalReference cuando creaste la preferencia
                long invitacionId = long.Parse(payment.ExternalReference);

                var invitacion = await _invitacionRepo.ObtenerPorIdAsync(invitacionId);
                if (invitacion == null) return;

                // Socio ya fue creado en AgregarInvitacionCasoDeUso
                var socio = await _usuarioRepo.ObtenerPorIdAsync(invitacion.UsuarioId ?? 0);
                if (socio == null) return;

                // Registrar ingreso
                await _agregarIngresoCaso.Ejecutar(new AgregarIngresoDTO
                {
                    GimnasioId = invitacion.GimnasioId,
                    UsuarioId = socio.Id,
                    MetodoPago = "MercadoPago",
                    Monto = (decimal)payment.TransactionAmount,
                    Observaciones = "Pago acreditado por Mercado Pago"
                });

                // ⭐⭐⭐ REUTILIZAMOS TU MÉTODO REAL PARA ENVIAR QR ⭐⭐⭐
                await _agregarInvitacionCaso.ProcesarInvitacionQrAsync(
                    new GenerarInvitacionDTO
                    {
                        Nombre = socio.Nombre,
                        Apellidos = socio.Apellido,
                        Email = socio.Email,
                        Telefono = socio.Telefono
                    },
                    socio.TokenRecuperacion,   // token ya generado cuando se creó el socio
                    invitacion
                );

                // Actualizamos el estado
                invitacion.Estado = "Pagado";
                await _invitacionRepo.ActualizarAsync(invitacion);
            }
            catch
            {
                // MP EXIGE 200 OK SIEMPRE
                return;
            }
        }
    }
}
