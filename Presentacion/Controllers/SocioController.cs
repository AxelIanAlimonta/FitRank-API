using FitRank_API.Application.DTOs.Logro;
using FitRank_API.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [ApiController]
    [Route("api/socios")]
    public class SocioController : ControllerBase
    {
        private readonly ISocioService _socioService;

        public SocioController(ISocioService socioService)
        {
            _socioService = socioService;
        }

        //GET /socios/{socioId}/logros
        [HttpGet("{socioId:int}/gimnasios/{gimnasioId:int}/logros")]
        public async Task<ActionResult<IReadOnlyList<LogroUsuarioDto>>> MisLogros([FromRoute] int socioId, [FromRoute] int gimnasioId)
        {
            var logros = await _socioService.MisLogrosAsync(socioId, gimnasioId);
            return Ok(logros);
        }
    }
}
