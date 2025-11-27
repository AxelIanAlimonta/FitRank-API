using FitRank_API.Application.CasosDeUso.Ingreso;
using FitRank_API.Application.DTOs.IngresoDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitRank_API.Presentacion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class IngresoController : ControllerBase
    {
        private readonly AgregarIngresoCasoDeUso _agregarCaso;
        private readonly ObtenerIngresosCasoDeUso _obtenerTodosCaso;
        private readonly ObtenerIngresoPorIdCasoDeUso _obtenerPorIdCaso;
        private readonly ObtenerIngresosPorGimnasioCasoDeUso _obtenerPorGimnasioCaso;
        private readonly EliminarIngresoCasoDeUso _eliminarCaso;

        public IngresoController(
            AgregarIngresoCasoDeUso agregarCaso,
            ObtenerIngresosCasoDeUso obtenerTodosCaso,
            ObtenerIngresoPorIdCasoDeUso obtenerPorIdCaso,
            ObtenerIngresosPorGimnasioCasoDeUso obtenerPorGimnasioCaso,
            EliminarIngresoCasoDeUso eliminarCaso)
        {
            _agregarCaso = agregarCaso;
            _obtenerTodosCaso = obtenerTodosCaso;
            _obtenerPorIdCaso = obtenerPorIdCaso;
            _obtenerPorGimnasioCaso = obtenerPorGimnasioCaso;
            _eliminarCaso = eliminarCaso;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodosLosIngresos()
        {
            try
            {
                var ingresos = await _obtenerTodosCaso.Ejecutar();
                return Ok(ingresos);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerIngresoPorId(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                var ingreso = await _obtenerPorIdCaso.Ejecutar(id);

                if (ingreso == null)
                    return NotFound(new { Mensaje = "Ingreso no encontrado." });

                return Ok(ingreso);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("gimnasio")]
        public async Task<IActionResult> ObtenerIngresoPorGimnasio()
        {
            try
            {
                var adminIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrWhiteSpace(adminIdString) || !long.TryParse(adminIdString, out var adminId))
                {
                    return BadRequest(new { Mensaje = "ID de administrador inválido en el token." });
                }

                var ingresos = await _obtenerPorGimnasioCaso.Ejecutar(adminId);
                return Ok(ingresos);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] AgregarIngresoDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var nuevo = await _agregarCaso.Ejecutar(dto);

                return Ok(new
                {
                    Mensaje = "Ingreso registrado correctamente.",
                    Ingreso = nuevo
                });
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
                var eliminado = await _eliminarCaso.Ejecutar(id);

                if (!eliminado)
                    return NotFound(new { Mensaje = "Ingreso no encontrado." });

                return Ok(new { Mensaje = "Ingreso eliminado correctamente." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
