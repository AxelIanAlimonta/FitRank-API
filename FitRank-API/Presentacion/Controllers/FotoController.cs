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
            var result = await _agregarFotoCasoDeUso.Ejecutar(dto);
            return Ok(result);
        }

        [HttpGet("por-socio/{socioId}")]
        public async Task<ActionResult<List<ObtenerFotoDTO>>> ObtenerPorSocio(long socioId)
        {
            var result = await _obtenerFotosPorSocioCasoDeUso.Ejecutar(socioId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            await _eliminarFotoCasoDeUso.Ejecutar(id);
            return NoContent();
        }
    }
}
