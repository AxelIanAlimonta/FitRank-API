using FitRank_API.Application.CasosDeUso.MercadoPago;
using FitRank_API.Application.DTOs.MercadoPago;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace FitRank_API.Presentacion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MercadoPagoController : ControllerBase
    {
        private readonly CrearPreferenciaMercadoPagoCasoDeUso _crearPreferenciaCaso;
        private readonly ProcesarWebhookPagoCasoDeUso _procesarWebhookCasoDeUso;


        public MercadoPagoController(
            CrearPreferenciaMercadoPagoCasoDeUso crearPreferenciaCaso,
            ProcesarWebhookPagoCasoDeUso procesarWebhookCasoDeUso
            )
        {
            _crearPreferenciaCaso = crearPreferenciaCaso;
            _procesarWebhookCasoDeUso = procesarWebhookCasoDeUso;
        }
        [HttpPost("crear-preferencia")]
        public async Task<IActionResult> CrearPreferencia([FromQuery] long invitacionId, [FromQuery] decimal monto, [FromQuery] string email)
        {
            try
            {
                var resultado = await _crearPreferenciaCaso.Ejecutar(monto, email, invitacionId);

                return Ok(new
                {
                    url = resultado.linkPago,     
                    qrImage = resultado.qrImage,
                    mensaje = "Preferencia creada correctamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al crear preferencia", detalle = ex.Message });
            }
        }
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            try
            {
                Console.WriteLine("====== WEBHOOK RECIBIDO ======");

                string topic = "";
                string id = "";

                // ✔ 1) Intentar leer FORM-DATA
                if (Request.HasFormContentType)
                {
                    topic = Request.Form["topic"];
                    id = Request.Form["id"];
                }

                // ✔ 2) Intentar leer QUERYSTRING (MUY IMPORTANTE)
                if (string.IsNullOrEmpty(topic))
                    topic = Request.Query["topic"];

                if (string.IsNullOrEmpty(id))
                    id = Request.Query["id"];

                // ✔ 3) Intentar leer JSON si todo lo anterior vino vacío
                if (string.IsNullOrEmpty(topic) || string.IsNullOrEmpty(id))
                {
                    using var reader = new StreamReader(Request.Body);
                    var bodyString = await reader.ReadToEndAsync();

                    if (!string.IsNullOrWhiteSpace(bodyString))
                    {
                        dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(bodyString);

                        topic = topic ?? json?.type ?? json?.topic;
                        id = id ?? json?.data?.id?.ToString() ?? json?.id?.ToString();
                    }
                }

                Console.WriteLine($"topic={topic}, id={id}");
                Console.WriteLine("================================");

                if (string.IsNullOrEmpty(topic) || string.IsNullOrEmpty(id))
                {
                    Console.WriteLine("⚠ Webhook sin datos válidos");
                    return Ok();
                }

                if (topic == "payment")
                {
                    long paymentId = long.Parse(id);
                    await _procesarWebhookCasoDeUso.Ejecutar(paymentId);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR EN WEBHOOK: " + ex.Message);
                return Ok();
            }
        }

        [HttpPost("renovar-cuota")]
        public async Task<IActionResult> RenovarCuota([FromBody] RenovarCuotaRequest req)
        {
            try
            {
                decimal monto = 30000;

                var resultado = await _crearPreferenciaCaso.Ejecutar(
                    monto,
                    req.Email,
                    req.SocioId
                );

                return Ok(new
                {
                    linkPago = resultado.linkPago,
                    mensaje = "Link generado correctamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al generar pago", detalle = ex.Message });
            }
        }





    }


}


    

