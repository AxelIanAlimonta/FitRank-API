using FitRank_API.Application.CasosDeUso.Ingreso;

using FitRank_API.Application.DTOs.IngresoDTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitRank_API.Presentacion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // 🔒 Solo administradores
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
            var ingresos = await _obtenerTodosCaso.Ejecutar();
            return Ok(ingresos);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerIngresoPorId(long id)
        {
            var ingreso = await _obtenerPorIdCaso.Ejecutar(id);

            if (ingreso == null)
                return NotFound(new { mensaje = "Ingreso no encontrado" });

            return Ok(ingreso);
        }
        [HttpGet("gimnasio")]
        public async Task<IActionResult> ObtenerIngresoPorGimnasio()
        {
            var adminIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (adminIdString == null)
                throw new Exception("No se encontró el UserId en el token.");

            var adminId = long.Parse(adminIdString);

            var ingresos = await _obtenerPorGimnasioCaso.Ejecutar(adminId);
            return Ok(ingresos);
        }



        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] AgregarIngresoDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           
            var adminId = long.Parse(User.FindFirstValue("id"));

            var nuevo = await _agregarCaso.Ejecutar(dto);

            return Ok(new
            {
                mensaje = "Ingreso registrado correctamente",
                ingreso = nuevo
            });
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            var eliminado = await _eliminarCaso.Ejecutar(id);

            if (!eliminado)
                return NotFound(new { mensaje = "Ingreso no encontrado" });

            return Ok(new { mensaje = "Ingreso eliminado correctamente" });
        }
    }
}
