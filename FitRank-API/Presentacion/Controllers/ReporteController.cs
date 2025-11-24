using FitRank_API.Application.CasosDeUso.ReporteCasosDeUso;
using FitRank_API.Application.DTOs.ReporteDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReporteController : ControllerBase
    {
        private readonly ObtenerTodosLosReportesDeGimnasioCasoDeUso _obtenerTodosLosReportesDeGimnasioCasoDeUso;
        private readonly AgregarReporteCasoDeUso _agregarReporteCasoDeUso;
        private readonly ActualizarReporteCasoDeUso _actualizarReporteCasoDeUso;
        private readonly EliminarReporteCasoDeUso _eliminarReporteCasoDeUso;
        private readonly ObtenerReportePorIdCasoDeUso _obtenerReportePorIdCasoDeUso;
        private readonly DesactivarReporteCasoDeUso _desactivarReporteCasoDeUso;
        private readonly ObtenerReportesPorUsuarioCasoDeUso _obtenerReportesPorUsuarioCasoDeUso;
        private readonly ObtenerReportesActivosDeUnGimnasioCasoDeUso _obtenerReportesActivosCasoDeUso;
        private readonly ObtenerReportesInactivosDeUnGimnasioCasoDeUso _obtenerReportesInactivosCasoDeUso;

        public ReporteController(
            ObtenerTodosLosReportesDeGimnasioCasoDeUso obtenerTodosLosReportesDeGimnasioCasoDeUso,
            AgregarReporteCasoDeUso agregarReporteCasoDeUso,
            ActualizarReporteCasoDeUso actualizarReporteCasoDeUso,
            EliminarReporteCasoDeUso eliminarReporteCasoDeUso,
            ObtenerReportePorIdCasoDeUso obtenerReportePorIdCasoDeUso,
            DesactivarReporteCasoDeUso desactivarReporteCasoDeUso,
            ObtenerReportesPorUsuarioCasoDeUso obtenerReportesPorUsuarioCasoDeUso,
            ObtenerReportesActivosDeUnGimnasioCasoDeUso obtenerReportesActivosCasoDeUso,
            ObtenerReportesInactivosDeUnGimnasioCasoDeUso obtenerReportesInactivosCasoDeUso
        )
        {
            _obtenerTodosLosReportesDeGimnasioCasoDeUso = obtenerTodosLosReportesDeGimnasioCasoDeUso;
            _agregarReporteCasoDeUso = agregarReporteCasoDeUso;
            _actualizarReporteCasoDeUso = actualizarReporteCasoDeUso;
            _eliminarReporteCasoDeUso = eliminarReporteCasoDeUso;
            _obtenerReportePorIdCasoDeUso = obtenerReportePorIdCasoDeUso;
            _desactivarReporteCasoDeUso = desactivarReporteCasoDeUso;
            _obtenerReportesPorUsuarioCasoDeUso = obtenerReportesPorUsuarioCasoDeUso;
            _obtenerReportesActivosCasoDeUso = obtenerReportesActivosCasoDeUso;
            _obtenerReportesInactivosCasoDeUso = obtenerReportesInactivosCasoDeUso;
        }

        [HttpGet("gimnasio/{gimnasioId}")]
        public async Task<IActionResult> ObtenerTodosLosReportesDeGimnasio(long gimnasioId)
        {
            try
            {
                var resultado = await _obtenerTodosLosReportesDeGimnasioCasoDeUso.Ejecutar(gimnasioId);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("agregar")]
        public async Task<IActionResult> AgregarReporte([FromBody] AgregarReporteDTO reporte)
        {
            if (reporte == null)
            {
                return BadRequest("El reporte no puede ser nulo.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var nuevoReporte = await _agregarReporteCasoDeUso.Ejecutar(reporte);
                return CreatedAtAction(nameof(ObtenerReportePorId), new { id = nuevoReporte.Id }, nuevoReporte);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("editar/{id:long}")]
        public async Task<IActionResult> ActualizarReporte(long id, [FromBody] ActualizarReporteDTO reporte)
        {
            if (reporte == null)
            {
                return BadRequest("El reporte no puede ser nulo.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != reporte.Id) return BadRequest("El ID del ejercicio no coincide con el ID proporcionado en la ruta.");

            try
            {
                var resultado = await _actualizarReporteCasoDeUso.Ejecutar(reporte);

                if (resultado == null)
                {
                    return NotFound($"El reporte con ID {id} no fue encontrado para actualizar.");
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al actualizar reporte");
            }
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> EliminarReporte(long id)
        {
            try
            {
                var resultado = await _eliminarReporteCasoDeUso.Ejecutar(id);

                if (!resultado)
                    return NotFound($"El reporte con ID {id} no fue encontrado para eliminar.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al eliminar reporte");
            }


        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> ObtenerReportePorId(long id)
        {
            var reporte = await _obtenerReportePorIdCasoDeUso.Ejecutar(id);

            if (reporte == null)
            {
                return NotFound($"El reporte con ID {id} no fue encontrado.");
            }

            return Ok(reporte); // Data = detalle del reporte
        }

        [HttpPut("desactivar/{id:long}")]
        public async Task<IActionResult> DesactivarReporte(long id)
        {
            try
            {
                var resultado = await _desactivarReporteCasoDeUso.Ejecutar(id);

                if (!resultado)
                    return NotFound($"El reporte con ID {id} no fue encontrado para desactivar.");

                return Ok("Reporte desactivado correctamente.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al desactivar reporte");
            }
        }

        // ReporteController.cs
        [HttpGet("usuario/{usuarioId:long}")]
        public async Task<IActionResult> ObtenerReportesPorUsuario(long usuarioId)
        {
            try
            {
                var resultado = await _obtenerReportesPorUsuarioCasoDeUso.Ejecutar(usuarioId);
                return Ok(resultado); // lista de ReporteDTO
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("gimnasio/{gimnasioId}/activos")]
        public async Task<IActionResult> ObtenerReportesActivos(long gimnasioId)
        {
            try
            {
                var resultado = await _obtenerReportesActivosCasoDeUso.Ejecutar(gimnasioId);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("gimnasio/{gimnasioId}/inactivos")]
        public async Task<IActionResult> ObtenerReportesInactivos(long gimnasioId)
        {
            try
            {
                var resultado = await _obtenerReportesInactivosCasoDeUso.Ejecutar(gimnasioId);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
