using FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso;
using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.DTOs.ProfesorDTOs;
using FitRank_API.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfesorController : ControllerBase
    {
        private readonly AgregarProfesorCasoDeUso _agregarProfesorCasoDeUso;
        private readonly ObtenerProfesorPorIdCasoDeUso _obtenerProfesorPorIdCasoDeUso;
        private readonly ActualizarProfesorCasoDeUso _actualizarProfesorCasoDeUso;
        private readonly EliminarProfesorCasoDeUso _eliminarProfesorCasoDeUso;
        private readonly ObtenerTodosLosProfesoresCasoDeUso _obtenerTodosLosProfesoresCasoDeUso;
        private readonly ObtenerTodosPorGimnasioCasoDeUso _obtenerTodosPorGimnasioCasoDeUso;
        private readonly ObtenerTodasLasRutinasPorProfesorCasoDeUso _obtenerTodasLasRutinasPorProfesorCasoDeUso;
        private readonly ObtenerEstadisticasProfesoresCasoDeUso _obtenerEstadisticasProfesoresCasoDeUso;

        public ProfesorController(
            AgregarProfesorCasoDeUso agregarProfesorCasoDeUso,
            ObtenerProfesorPorIdCasoDeUso obtenerProfesorPorIdCasoDeUso,
            ActualizarProfesorCasoDeUso actualizarProfesorCasoDeUso,
            ObtenerTodosLosProfesoresCasoDeUso obtenerTodosLosProfesoresCasoDeUso,
            EliminarProfesorCasoDeUso eliminarProfesorCasoDeUso, ObtenerTodasLasRutinasPorProfesorCasoDeUso obtenerTodasLasRutinasPorProfesorCasoDeUso, ObtenerEstadisticasProfesoresCasoDeUso obtenerEstadisticasProfesoresCasoDeUso)
        {
            _agregarProfesorCasoDeUso = agregarProfesorCasoDeUso;
            _obtenerProfesorPorIdCasoDeUso = obtenerProfesorPorIdCasoDeUso;
            _obtenerTodosLosProfesoresCasoDeUso = obtenerTodosLosProfesoresCasoDeUso;
            _actualizarProfesorCasoDeUso = actualizarProfesorCasoDeUso;
            _eliminarProfesorCasoDeUso = eliminarProfesorCasoDeUso;
            _obtenerTodasLasRutinasPorProfesorCasoDeUso = obtenerTodasLasRutinasPorProfesorCasoDeUso;
            _obtenerEstadisticasProfesoresCasoDeUso = obtenerEstadisticasProfesoresCasoDeUso;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodosAsync()
        {
            var profesores = await _obtenerTodosLosProfesoresCasoDeUso.Ejecutar();
            return Ok(profesores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            var profesor = await _obtenerProfesorPorIdCasoDeUso.Ejecutar(id);
            if (profesor == null)
            {
                return NotFound();
            }
            return Ok(profesor);

        }
        [HttpPost]
        public async Task<IActionResult> AgregarAsync([FromBody] AgregarProfesorDTO profesorDTO)
        {
            try
            {

                var nuevoProfesor = await _agregarProfesorCasoDeUso.Ejecutar(profesorDTO);
                return Ok(nuevoProfesor);

            }
            catch (Exception ex)
            {
                if (ex.Message == "EMAIL_DUPLICADO")
                    return BadRequest(new { mensaje = "Ya existe un profesor con este email." });

                if (ex.Message == "DNI_DUPLICADO")
                    return BadRequest(new { mensaje = "Ya existe un profesor con este DNI." });

                return StatusCode(500, new { mensaje = "Error interno del servidor." });
            }

        }
        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> Actualizar(long id, ActualizarProfesorDTO dto)
        {
            try
            {
                var actualizado = await _actualizarProfesorCasoDeUso.Ejecutar(id, dto);

                if (actualizado == null)
                    return NotFound(new { mensaje = "Profesor no encontrado." });

                return Ok(actualizado);
            }
            catch (Exception ex)
            {
                if (ex.Message == "EMAIL_DUPLICADO")
                    return BadRequest(new { mensaje = "Ya existe un profesor con este email." });

                if (ex.Message == "DNI_DUPLICADO")
                    return BadRequest(new { mensaje = "Ya existe un profesor con este DNI." });

                return StatusCode(500, new { mensaje = "Error interno del servidor." });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarAsync(long id)
        {
            var eliminado = await _eliminarProfesorCasoDeUso.Ejecutar(id);
            if (!eliminado)
            {
                return NotFound();
            }
            return NoContent();
        }
        [HttpGet("gimnasio/{gimnasioId}")]
        public async Task<IActionResult> ObtenerPorGimnasio(long gimnasioId)
        {
            var profesores = await _obtenerTodosPorGimnasioCasoDeUso.Ejecutar(gimnasioId);
            return Ok(profesores);
        }

        [HttpGet("profesor/{usuarioId}")]
        public async Task<IActionResult> ObtenerRutinasPorProfesor(long usuarioId)
        {
            var rutinas = await _obtenerTodasLasRutinasPorProfesorCasoDeUso.Ejecutar(usuarioId);

            if (rutinas == null || !rutinas.Any())
                return NotFound(new { mensaje = "No se encontraron rutinas para este profesor." });




            return Ok(rutinas);
        }

        [HttpGet("estadisticas")]
        public async Task<IActionResult> ObtenerEstadisticas()
        {
            var resultado = await _obtenerEstadisticasProfesoresCasoDeUso.Ejecutar();

            if (resultado == null)
                return NotFound(new { mensaje = "No hay datos de estadísticas de profesores." });

            return Ok(resultado);
        }
    }
}
