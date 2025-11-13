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
        private readonly ProcesarPagoMercadoPagoCasoDeUso _procesarPagoCaso;

        public MercadoPagoController(
            CrearPreferenciaMercadoPagoCasoDeUso crearPreferenciaCaso,
            ProcesarPagoMercadoPagoCasoDeUso procesarPagoCaso)
        {
            _crearPreferenciaCaso = crearPreferenciaCaso;
            _procesarPagoCaso = procesarPagoCaso;
        }

 
        // Crea la preferencia de pago y devuelve la URL del checkout
        [HttpPost("crear-preferencia")]
        public async Task<IActionResult> CrearPreferencia([FromQuery] long invitacionId, [FromQuery] decimal monto, [FromQuery] string email)
        {
            try
            {
                var url = await _crearPreferenciaCaso.Ejecutar(monto, email, invitacionId);
                return Ok(new { url });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al crear la preferencia", detalle = ex.Message });
            }
        }

        // Mercado Pago llama a esta ruta cuando el pago cambia de estado
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] JObject body)
        {
            try
            {
                await _procesarPagoCaso.Ejecutar(body);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Webhook Error] {ex.Message}");
                return Ok(); 
            }
        }
    }
}
