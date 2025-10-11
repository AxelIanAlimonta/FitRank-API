using FitRank_API.Application.DTOs.Rutina;
using FitRank_API.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RutinaController : ControllerBase
    {
        private readonly IRutinaServicio _rutinaServicio;

        public RutinaController(IRutinaServicio rutinaServicio)
        {
            _rutinaServicio = rutinaServicio;
        }

        [HttpPost]
        public async Task<IActionResult> CrearRutina([FromBody] CrearRutinaDTO dto)
        {
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var rutinaCreada = await _rutinaServicio.CrearRutinaAsync(dto);
                return Ok(new
                {
                    Mensaje = "Rutina creada exitosamente",
                    Rutina = rutinaCreada
                });
            }
            catch
            {
                return BadRequest("No se pudo crear la rutina");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerRutina(int id)
        {
            var rutina = await _rutinaServicio.ObtenerRutinaAsync(id);
            if (rutina == null)
                return NotFound("Rutina no encontrada");

            return Ok(rutina);
        }

        [HttpGet]
        public async Task<IActionResult> ListarRutinas()
        {
            var rutinas = await _rutinaServicio.ListarRutinasAsync();
            return Ok(rutinas);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarRutina(int id, [FromBody] ActualizarRutinaDTO dto)
        {
            try
            {
                var rutinaActualizada = await _rutinaServicio.ActualizarRutinaAsync(id, dto);
                if (rutinaActualizada == null)
                    return NotFound("Rutina no encontrada");

                return Ok(new
                {
                    Mensaje = "Rutina actualizada correctamente",
                    Rutina = rutinaActualizada
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"No fue posible actualizar la rutina: {ex.Message}");
            }
            /*
            catch
            {
                return BadRequest("No fue posible actualizar la rutina");
            }
            */
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarRutina(int id)
        {
            try
            {
                var exito = await _rutinaServicio.EliminarRutinaAsync(id);
                if (!exito)
                    return NotFound("Rutina no encontrado");

                return Ok("Rutina eliminada correctamente");
            }
            catch
            {
                return BadRequest("No fue posible eliminar la rutina");
            }
        }
    }
}
