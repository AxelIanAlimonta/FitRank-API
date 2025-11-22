using FitRank_API.Application.CasosDeUso.MercadoPago;
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
        public async Task<IActionResult> Webhook([FromBody] JObject body)
        {
            try
            {
                Console.WriteLine("== WEBHOOK RECIBIDO ==");
                Console.WriteLine(body.ToString());

                string? tipo = (string?)body["type"];
                string? action = (string?)body["action"];

                string? id =
                    (string?)body["data"]?["id"] ??
                    (string?)body["data_id"] ??
                    (string?)body["id"];

                Console.WriteLine($"type={tipo}, action={action}, id={id}");

                if (string.IsNullOrEmpty(id))
                    return Ok();

                
                if (tipo?.StartsWith("payment") == true ||
                    action?.StartsWith("payment") == true)
                {
                    await _procesarWebhookCasoDeUso.Ejecutar(body);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR EN WEBHOOK: " + ex);
                return Ok();
            }
        }

    }


}


    

