using FitRank_API.Application.DTOs.Logro;
using FitRank_API.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [ApiController]
    [Route("api/gimnasios/{gimnasioId:int}/logros")]
    public class GimnasioLogrosController : ControllerBase
    {
        private readonly IGimnasioService _gimnasioService;

        public GimnasioLogrosController(IGimnasioService gimnasioService)
        {
            _gimnasioService = gimnasioService;
        }

        //GET /gimnasios/{gimnasioId}/logros
        [HttpGet("activos")]
        public async Task<ActionResult<IReadOnlyList<LogroDto>>> GetLogrosPorGimnasio([FromRoute] int gimnasioId)
        {
            var logros = await _gimnasioService.ListarLogrosActivosAsync(gimnasioId);
            return Ok(logros);
        }

        //POST /otorgar
        [HttpPost("socios/{socioId}/logros/{logroId}/otorgar")]
        public async Task<ActionResult> OtorgarLogroASocio([FromRoute] int gimnasioId, [FromRoute] int socioId, [FromRoute] int logroId)
        {
            await _gimnasioService.OtorgarLogroAsync(socioId, logroId, gimnasioId);
            return NoContent();
        }
    }
}
