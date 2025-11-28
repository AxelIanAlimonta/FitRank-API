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
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                var maquina = await _obtenerMaquinaPorIdCasoDeUso.Ejecutar(id);
                if (maquina == null)
                {
                    return NotFound(new { Mensaje = "Máquina no encontrada." });
                }
                return Ok(maquina);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] AgregarMaquinaDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var gimnasioClaim = User.FindFirst(ClaimTypes.GroupSid);

                if (gimnasioClaim == null || string.IsNullOrWhiteSpace(gimnasioClaim.Value))
                    return Unauthorized(new { Mensaje = "No se encontró el gimnasio en el token." });

                if (!long.TryParse(gimnasioClaim.Value, out var gimnasioId) || gimnasioId <= 0)
                    return BadRequest(new { Mensaje = "El ID del gimnasio en el token es inválido." });

                var result = await _agregarMaquinaCasoDeUso.Ejecutar(dto, gimnasioId);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarMaquinaDTO actualizarMaquinaDTO)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            if (actualizarMaquinaDTO == null)
            {
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });
            }

            if (id != actualizarMaquinaDTO.Id)
            {
                return BadRequest(new { Mensaje = "El ID de la URL no coincide con el ID de la máquina." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var maquinaActualizada = await _actualizarMaquinaCasoDeUso.Ejecutar(actualizarMaquinaDTO);
                if (maquinaActualizada == null)
                {
                    return NotFound(new { Mensaje = "Máquina no encontrada." });
                }
                return Ok(maquinaActualizada);
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
                var exito = await _eliminarMaquinaCasoDeUso.Ejecutar(id);
                if (!exito)
                {
                    return NotFound(new { Mensaje = "Máquina no encontrada." });
                }
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("{id}/detalles")]
        public async Task<IActionResult> ObtenerDetalles(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                var result = await _obtenerMaquinaDetalleCasoDeUso.Ejecutar(id);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
