using FitRank_API.Application.CasosDeUso.FotoCasosDeUso;
using FitRank_API.Application.DTOs.FotoDTOs;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FotoController : ControllerBase
    {
        private readonly AgregarFotoCasoDeUso _agregarFotoCasoDeUso;
        private readonly ObtenerFotosPorSocioCasoDeUso _obtenerFotosPorSocioCasoDeUso;
        private readonly EliminarFotoCasoDeUso _eliminarFotoCasoDeUso;

        public FotoController(
            AgregarFotoCasoDeUso agregarFotoCasoDeUso,
            ObtenerFotosPorSocioCasoDeUso obtenerFotosPorSocioCasoDeUso,
            EliminarFotoCasoDeUso eliminarFotoCasoDeUso)
        {
            _agregarFotoCasoDeUso = agregarFotoCasoDeUso;
            _obtenerFotosPorSocioCasoDeUso = obtenerFotosPorSocioCasoDeUso;
            _eliminarFotoCasoDeUso = eliminarFotoCasoDeUso;
        }

        [HttpPost("agregar")]
        public async Task<ActionResult<ObtenerFotoDTO>> Agregar([FromBody] AgregarFotoDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _agregarFotoCasoDeUso.Ejecutar(dto);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("por-socio/{socioId}")]
        public async Task<ActionResult<List<ObtenerFotoDTO>>> ObtenerPorSocio(long socioId)
        {
            if (socioId <= 0)
                return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

            try
            {
                var result = await _obtenerFotosPorSocioCasoDeUso.Ejecutar(socioId);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                await _eliminarFotoCasoDeUso.Ejecutar(id);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
