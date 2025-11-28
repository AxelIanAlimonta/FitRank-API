using System.Security.Claims;
using FitRank_API.Application.CasosDeUso.MedidaCorporalCasosDeUso;
using FitRank_API.Application.DTOs.MedidaCorporalDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Profesor,Socio")]
    public class MedidaCorporalController : ControllerBase
    {
        private readonly AgregarMedidaCorporalCasoDeUso _agregarCasoDeUso;
        private readonly ActualizarMedidaCorporalCasoDeUso _actualizarCasoDeUso;
        private readonly ObtenerMedidaCorporalPorIdCasoDeUso _obtenerPorIdCasoDeUso;
        private readonly ObtenerMedidasPorSocioCasoDeUso _obtenerPorSocioCasoDeUso;
        private readonly EliminarMedidaCorporalCasoDeUso _eliminarCasoDeUso;

        public MedidaCorporalController(
            AgregarMedidaCorporalCasoDeUso agregarCasoDeUso,
            ActualizarMedidaCorporalCasoDeUso actualizarCasoDeUso,
            ObtenerMedidaCorporalPorIdCasoDeUso obtenerPorIdCasoDeUso,
            ObtenerMedidasPorSocioCasoDeUso obtenerPorSocioCasoDeUso,
            EliminarMedidaCorporalCasoDeUso eliminarCasoDeUso)
        {
            _agregarCasoDeUso = agregarCasoDeUso;
            _actualizarCasoDeUso = actualizarCasoDeUso;
            _obtenerPorIdCasoDeUso = obtenerPorIdCasoDeUso;
            _obtenerPorSocioCasoDeUso = obtenerPorSocioCasoDeUso;
            _eliminarCasoDeUso = eliminarCasoDeUso;
        }

        [HttpPost("agregar")]
        public async Task<IActionResult> Agregar([FromBody] AgregarMedidaCorporalDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _agregarCasoDeUso.Ejecutar(dto);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPut("actualizar")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarMedidaCorporalDTO dto)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Id != id)
                return BadRequest(new { Mensaje = "El ID de la URL no coincide con el ID de la medida corporal." });

            try
            {
                var result = await _actualizarCasoDeUso.Ejecutar(dto);
                if (result == null)
                    return NotFound(new { Mensaje = $"No se encontró ninguna medida corporal con ID {dto.Id}." });
                return Ok(result);
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
                var result = await _obtenerPorIdCasoDeUso.Ejecutar(id);
                if (result == null)
                    return NotFound(new { Mensaje = $"No se encontró ninguna medida corporal con ID {id}." });
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("historial")]
        public async Task<IActionResult> ObtenerPorSocio([FromQuery] long? socioId = null)
        {
            try
            {
                var rol = User.FindFirst(ClaimTypes.Role)?.Value;
                var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (rol == "Socio")
                {
                    if (usuarioIdClaim == null || string.IsNullOrWhiteSpace(usuarioIdClaim.Value))
                        return Unauthorized(new { Mensaje = "No se encontró el ID del usuario en el token." });

                    if (!long.TryParse(usuarioIdClaim.Value, out var usuarioId) || usuarioId <= 0)
                        return BadRequest(new { Mensaje = "El ID del usuario en el token es inválido." });

                    socioId = usuarioId;
                }

                if (socioId == null || socioId.Value <= 0)
                    return BadRequest(new { Mensaje = "Debe indicar un socioId válido o estar autenticado como socio." });

                var result = await _obtenerPorSocioCasoDeUso.Ejecutar(socioId.Value);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                var eliminado = await _eliminarCasoDeUso.Ejecutar(id);
                if (!eliminado)
                    return NotFound(new { Mensaje = "Medición no encontrada o no autorizada." });
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
