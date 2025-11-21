using System.Security.Claims;
using FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso;
using FitRank_API.Application.DTOs.MaquinaDTOs;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaquinaController : ControllerBase
    {
        private readonly ObtenerMaquinasCasoDeUso _obtenerMaquinasCasoDeUso;
        private readonly AgregarMaquinaCasoDeUso _agregarMaquinaCasoDeUso;
        private readonly ActualizarMaquinaCasoDeUso _actualizarMaquinaCasoDeUso;
        private readonly EliminarMaquinaCasoDeUso _eliminarMaquinaCasoDeUso;
        private readonly ObtenerMaquinaPorIdCasoDeUso _obtenerMaquinaPorIdCasoDeUso;
        private readonly ObtenerMaquinaDetalleCasoDeUso _obtenerMaquinaDetalleCasoDeUso;

      
        public MaquinaController(
            ObtenerMaquinasCasoDeUso obtenerMaquinasCasoDeUso,
            AgregarMaquinaCasoDeUso agregarMaquinaCasoDeUso,
            ActualizarMaquinaCasoDeUso actualizarMaquinaCasoDeUso,
            EliminarMaquinaCasoDeUso eliminarMaquinaCasoDeUso,
            ObtenerMaquinaPorIdCasoDeUso obtenerMaquinaPorIdCasoDeUso,
            ObtenerMaquinaDetalleCasoDeUso obtenerMaquinaDetalleCasoDeUso)
        {
            _obtenerMaquinasCasoDeUso = obtenerMaquinasCasoDeUso;
            _agregarMaquinaCasoDeUso = agregarMaquinaCasoDeUso;
            _actualizarMaquinaCasoDeUso = actualizarMaquinaCasoDeUso;
            _eliminarMaquinaCasoDeUso = eliminarMaquinaCasoDeUso;
            _obtenerMaquinaPorIdCasoDeUso = obtenerMaquinaPorIdCasoDeUso;
            _obtenerMaquinaDetalleCasoDeUso = obtenerMaquinaDetalleCasoDeUso;
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            try
            {
                var maquinas = await _obtenerMaquinasCasoDeUso.Ejecutar();
                return Ok(maquinas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error de servidor.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            var maquina = await _obtenerMaquinaPorIdCasoDeUso.Ejecutar(id);
            if (maquina == null)
            {
                return NotFound();
            }
            return Ok(maquina);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] AgregarMaquinaDTO dto)
        {
            
            var gimnasioClaim = User.FindFirst(ClaimTypes.GroupSid);

            if (gimnasioClaim == null)
                return Unauthorized("No se encontró el gimnasio en el token.");

            long gimnasioId = long.Parse(gimnasioClaim.Value);

            // Ejecutar caso de uso
            var result = await _agregarMaquinaCasoDeUso.Ejecutar(dto, gimnasioId);

            return Ok(result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarMaquinaDTO actualizarLogroDTO)
        {
            if (actualizarLogroDTO == null)
            {
                return BadRequest("El objeto no puede ser nulo.");
            }

            if (id != actualizarLogroDTO.Id)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del cuerpo de la solicitud.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var maquinaActualizada = await _actualizarMaquinaCasoDeUso.Ejecutar(actualizarLogroDTO);
                if (maquinaActualizada == null)
                {
                    return NotFound("Maquina no encontrada.");
                }
                return Ok(maquinaActualizada);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error de servidor.");
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            try
            {
                var exito = await _eliminarMaquinaCasoDeUso.Ejecutar(id);
                if (!exito)
                {
                    return NotFound("Maquina no encontrada.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error de servidor.");
            }
        }

        [HttpGet("{id}/detalles")]
        public async Task<IActionResult> ObtenerDetalles(long id)
        {
            var result = await _obtenerMaquinaDetalleCasoDeUso.Ejecutar(id);
            return Ok(result);
        }

    }
}
