using FitRank_API.Application.CasosDeUso.ReporteCasosDeUso;
using FitRank_API.Application.DTOs.ReporteDTOs;
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
            if (gimnasioId <= 0)
                return BadRequest(new { Mensaje = "El ID del gimnasio debe ser mayor a cero." });

            try
            {
                var resultado = await _obtenerTodosLosReportesDeGimnasioCasoDeUso.Ejecutar(gimnasioId);
                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost("agregar")]
        public async Task<IActionResult> AgregarReporte([FromBody] AgregarReporteDTO reporte)
        {
            if (reporte == null)
                return BadRequest(new { Mensaje = "El reporte no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var nuevoReporte = await _agregarReporteCasoDeUso.Ejecutar(reporte);
                return CreatedAtAction(nameof(ObtenerReportePorId), new { id = nuevoReporte!.Id }, nuevoReporte);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPut("editar/{id:long}")]
        public async Task<IActionResult> ActualizarReporte(long id, [FromBody] ActualizarReporteDTO reporte)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID del reporte debe ser mayor a cero." });

            if (reporte == null)
                return BadRequest(new { Mensaje = "El reporte no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != reporte.Id)
                return BadRequest(new { Mensaje = "El ID del reporte no coincide con el ID proporcionado en la ruta." });

            try
            {
                var resultado = await _actualizarReporteCasoDeUso.Ejecutar(reporte);

                if (resultado == null)
                    return NotFound(new { Mensaje = $"El reporte con ID {id} no fue encontrado." });

                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> EliminarReporte(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID del reporte debe ser mayor a cero." });

            try
            {
                var resultado = await _eliminarReporteCasoDeUso.Ejecutar(id);

                if (!resultado)
                    return NotFound(new { Mensaje = $"El reporte con ID {id} no fue encontrado." });

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> ObtenerReportePorId(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID del reporte debe ser mayor a cero." });

            try
            {
                var reporte = await _obtenerReportePorIdCasoDeUso.Ejecutar(id);

                if (reporte == null)
                    return NotFound(new { Mensaje = $"El reporte con ID {id} no fue encontrado." });

                return Ok(reporte);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPut("desactivar/{id:long}")]
        public async Task<IActionResult> DesactivarReporte(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID del reporte debe ser mayor a cero." });

            try
            {
                var resultado = await _desactivarReporteCasoDeUso.Ejecutar(id);

                if (!resultado)
                    return NotFound(new { Mensaje = $"El reporte con ID {id} no fue encontrado." });

                return Ok(new { Mensaje = "Reporte desactivado correctamente." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("usuario/{usuarioId:long}")]
        public async Task<IActionResult> ObtenerReportesPorUsuario(long usuarioId)
        {
            if (usuarioId <= 0)
                return BadRequest(new { Mensaje = "El ID del usuario debe ser mayor a cero." });

            try
            {
                var resultado = await _obtenerReportesPorUsuarioCasoDeUso.Ejecutar(usuarioId);
                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("gimnasio/{gimnasioId}/activos")]
        public async Task<IActionResult> ObtenerReportesActivos(long gimnasioId)
        {
            if (gimnasioId <= 0)
                return BadRequest(new { Mensaje = "El ID del gimnasio debe ser mayor a cero." });

            try
            {
                var resultado = await _obtenerReportesActivosCasoDeUso.Ejecutar(gimnasioId);
                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("gimnasio/{gimnasioId}/inactivos")]
        public async Task<IActionResult> ObtenerReportesInactivos(long gimnasioId)
        {
            if (gimnasioId <= 0)
                return BadRequest(new { Mensaje = "El ID del gimnasio debe ser mayor a cero." });

            try
            {
                var resultado = await _obtenerReportesInactivosCasoDeUso.Ejecutar(gimnasioId);
                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
