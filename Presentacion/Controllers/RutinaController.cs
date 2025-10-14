
using FitRank_API.Application.DTOs.RutinaNamespace;
using FitRank_API.Application.DTOs.RutinaNameSpace;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RutinaController : ControllerBase
    {
        private readonly IRutinaService _rutinaService;

        public RutinaController(IRutinaService rutinaService)
        {
            _rutinaService = rutinaService;
        }

        //crearRutina
        [HttpPost] // api/rutina
        public async Task<IActionResult> CrearRutina([FromBody] CrearRutinaDTO nuevaRutina)
        {
            if (nuevaRutina == null)
            {
                return BadRequest(new { mensaje = "Los datos de la rutina no son válidos." });
            }

            var rutinaCreada = await _rutinaService.CrearRutinaAsync(nuevaRutina);

            if (rutinaCreada == null)
            {
                return BadRequest(new { mensaje = "No se pudo crear la rutina." });
            }

            return Ok(new { mensaje = $"Rutina '{rutinaCreada.Nombre}' creada correctamente." });
        }

        //listar rutinas
        [HttpGet] // api/rutina
        public async Task<IActionResult> ListarRutinas()
        {
            var rutinas = await _rutinaService.ListarRutinasAsync();

            if (rutinas == null || !rutinas.Any())
            {
                return NotFound(new { mensaje = "No se encontraron rutinas en la base de datos." });
            }

            return Ok(rutinas);
        }

        //listar rutinas por usuario (usuarioId)

        [HttpGet("usuario/{usuarioId}")] // api/rutina/usuario/2
        public async Task<IActionResult> ListarRutinasPorUsuario(int usuarioId)
        {
            var rutinas = await _rutinaService.ListarRutinasPorUsuarioAsync(usuarioId);

            if (rutinas == null || !rutinas.Any())
            {
                return NotFound(new { mensaje = "No se encontraron rutinas para el usuario con ID {usuarioId}" });
            }

            return Ok(rutinas);
        }

        //obtener rutina por ID (especifica)
        [HttpGet("{id}")] // api/rutina/2
        public async Task<IActionResult> ObtenerRutinaPorId(int id)
        {
            var rutina = await _rutinaService.ObtenerRutinaPorIdAsync(id);

            if (rutina == null)
            {
                return NotFound(new { mensaje = $"No se encontró ninguna rutina con ID {id}" });
            }

            return Ok(rutina);
        }

        //editarRutina
        [HttpPut("{id}")] // api/rutina/2
        public async Task<IActionResult> EditarRutina(int id, [FromBody] EditarRutinaDTO rutinaActualizada)
        {
            if (rutinaActualizada == null)
            {
                return BadRequest(new { mensaje = "Los datos de la rutina no son válidos o el ID no coincide." });
            }

            var rutinaEditada = await _rutinaService.EditarRutinaAsync(id, rutinaActualizada);

            if (rutinaEditada == null)
            {
                return NotFound(new { mensaje = $"No se encontró la rutina con ID {id} para actualizar." });
            }

            return Ok(rutinaEditada);
        }

        //eliminarRutina
        [HttpDelete("{id}")] // api/rutina/2
        public async Task<IActionResult> EliminarRutina(int id)
        {
            var eliminado = await _rutinaService.EliminarRutinaAsync(id);

            if (!eliminado)
            {
                return NotFound(new { mensaje = $"No se encontró ninguna rutina con ID {id} para eliminar." });
            }

            return Ok(new { mensaje = $"Rutina con ID {id} eliminada correctamente." });
        }
    }
}
