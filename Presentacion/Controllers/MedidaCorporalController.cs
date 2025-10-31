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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var socioId = long.Parse(User.FindFirst("id")!.Value);

            var result = await _agregarCasoDeUso.Ejecutar(dto);
            return Ok(result);
        }


        [HttpPut("actualizar")]
        public async Task<IActionResult> Actualizar([FromBody] ActualizarMedidaCorporalDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var socioId = long.Parse(User.FindFirst("id")!.Value);

            var result = await _actualizarCasoDeUso.Ejecutar(dto);
            if (result == null)
                return NotFound(new { Mensaje = "Medición no encontrada o no autorizada" });

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            var socioId = long.Parse(User.FindFirst("id")!.Value);
            var result = await _obtenerPorIdCasoDeUso.Ejecutar(id);

            if (result == null)
                return NotFound(new { Mensaje = "Medición no encontrada o no autorizada" });

            return Ok(result);
        }

        [HttpGet("historial")]
        public async Task<IActionResult> ObtenerPorSocio([FromQuery] long? socioId = null)
        {
            var rol = User.FindFirst("rol")?.Value;
            var usuarioId = long.Parse(User.FindFirst("id")!.Value);

            if (rol == "Socio")
                socioId = usuarioId;

            if (socioId == null)
                return BadRequest(new { Mensaje = "Debe indicar el socioId o estar autenticado como socio." });

            var result = await _obtenerPorSocioCasoDeUso.Ejecutar(socioId.Value);
            return Ok(result);
        }


        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            var socioId = long.Parse(User.FindFirst("id")!.Value);
            var rol = User.FindFirst("rol")?.Value;

            var eliminado = await _eliminarCasoDeUso.Ejecutar(id);
            if (!eliminado)
                return NotFound(new { Mensaje = "Medición no encontrada o no autorizada" });

            return Ok(new { Mensaje = "Medición eliminada correctamente" });
        }
    }
}
