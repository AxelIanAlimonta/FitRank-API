using FitRank_API.Application.DTOs.Logro;
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

        //GET /logros
        [HttpGet]
        public async Task<IActionResult> GetLogrosActivos()
        {
            var logros = await _logroService.ListarAsync();
            return Ok(logros);
        }

        //GET /logros/{id}
        [HttpGet("{logroId:int}")]
        public async Task<IActionResult> ObtenerLogroPorId([FromRoute] int logroId)
        {
            var logro = await _logroService.ObtenerPorIdAsync(logroId);
            if (logro == null)
            {
                return NotFound();
            }
            return Ok(logro);
        }

        //POST /logros
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] LogroCreateDto logroDto)
        {
            if (logroDto == null)
            {
                return BadRequest("El logro no puede ser nulo.");
            }
            var logroId = await _logroService.CrearLogroAsync(logroDto);
            return CreatedAtAction(nameof(ObtenerLogroPorId), new { logroId = logroId }, logroDto);
        }
    }
}
