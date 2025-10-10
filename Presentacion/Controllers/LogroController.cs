using FitRank_API.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogroController : ControllerBase
    {
        private readonly ILogroService _logroService;

        public LogroController(ILogroService logroService)
        {
            _logroService = logroService;
        }

        //GET logros activos
        [HttpGet]
        public async Task<IActionResult> GetLogrosActivos(CancellationToken ct)
        {
            var logros = await _logroService.ListarActivosAsync(ct);
            return Ok(logros);
        }

        //GET Historial por socio
        [HttpGet("historial")]
        public async Task<IActionResult> GetMisLogros([FromQuery] int socioId, CancellationToken ct)
        {
            var logros = await _logroService.MisLogrosAsync(socioId, ct);
            return Ok(logros);
        }

        //POST Otorgar logro
        [HttpPost("{logroId:int}/otorgar")]
        public async Task<IActionResult> OtorgarLogro([FromRoute] int logroId, [FromQuery] int socioId, CancellationToken ct)
        {
            if (socioId <= 0 || logroId <= 0)
            {
                return BadRequest("Los IDs de socio y logro deben ser mayores que cero.");
            }
            try
            {
                await _logroService.OtorgarSiNoExisteAsync(socioId, logroId, ct);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
