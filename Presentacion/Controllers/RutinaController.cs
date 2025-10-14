
using FitRank_API.Application.DTOs.RutinaNamespace;
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
        public async Task<IActionResult> EditarRutina(int id, [FromBody] RutinaDTO rutinaActualizada)
        {
            if (rutinaActualizada == null || id != rutinaActualizada.Id)
                return BadRequest(new { mensaje = "Los datos de la rutina no son válidos o el ID no coincide." });

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

        //agregarBloque
        [HttpPost("{rutinaId}/bloque")] // api/rutina/bloque
        public async Task<IActionResult> AgregarBloque(int rutinaId, [FromBody] BloqueDTO nuevoBloque)
        {
            if (nuevoBloque == null)
            {
                return BadRequest(new { mensaje = "El bloque proporcionado no es válido." });
            }

            var bloqueAgregado = await _rutinaService.AgregarBloqueAsync(rutinaId, nuevoBloque);

            if (bloqueAgregado == null)
            {
                return NotFound(new { mensaje = $"No se encontró la rutina con ID {rutinaId} para agregar el bloque." });
            }

            return Ok(bloqueAgregado);
        }

        //obtenerBloquePorId
        [HttpGet("bloque/{id}")] // api/rutina/bloque/2
        public async Task<IActionResult> ObtenerBloquePorId(int id)
        {
            var bloque = await _rutinaService.ObtenerBloquePorIdAsync(id);

            if (bloque == null)
            {
                return NotFound(new { mensaje = $"No se encontró el bloque con ID {id}." });
            }

            return Ok(bloque);
        }

        //editarBloque
        [HttpPut("bloque/{id}")] // api/rutina/bloque/5
        public async Task<IActionResult> EditarBloque(int id, [FromBody] BloqueDTO bloqueActualizado)
        {
            if (bloqueActualizado == null || id != bloqueActualizado.Id)
            {
                return BadRequest(new { mensaje = "Los datos del bloque no son válidos o el ID no coincide." });
            }

            var bloqueEditado = await _rutinaService.EditarBloqueAsync(id, bloqueActualizado);

            if (bloqueEditado == null)
            {
                return NotFound(new { mensaje = $"No se encontró el bloque con ID {id} para actualizar." });
            }

            return Ok(bloqueEditado);
        }

        //eliminarBloque
        [HttpDelete("bloque/{id}")] // api/rutina/bloque/5
        public async Task<IActionResult> EliminarBloque(int id)
        {
            var bloqueEliminado = await _rutinaService.EliminarBloqueAsync(id);

            if (!bloqueEliminado)
            {
                return NotFound(new { mensaje = $"No se encontró el bloque con ID {id} para eliminar." });
            }
            return Ok(new { mensaje = $"El bloque con ID {id} fue eliminado correctamente." });
        }
    }
}
