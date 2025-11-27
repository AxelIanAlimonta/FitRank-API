using FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso;
using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.DTOs.ProfesorDTOs;
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
            EliminarProfesorCasoDeUso eliminarProfesorCasoDeUso,
            ObtenerTodosPorGimnasioCasoDeUso obtenerTodosPorGimnasioCasoDeUso,
            ObtenerTodasLasRutinasPorProfesorCasoDeUso obtenerTodasLasRutinasPorProfesorCasoDeUso,
            ObtenerEstadisticasProfesoresCasoDeUso obtenerEstadisticasProfesoresCasoDeUso)
        {
            _agregarProfesorCasoDeUso = agregarProfesorCasoDeUso;
            _obtenerProfesorPorIdCasoDeUso = obtenerProfesorPorIdCasoDeUso;
            _obtenerTodosLosProfesoresCasoDeUso = obtenerTodosLosProfesoresCasoDeUso;
            _actualizarProfesorCasoDeUso = actualizarProfesorCasoDeUso;
            _eliminarProfesorCasoDeUso = eliminarProfesorCasoDeUso;
            _obtenerTodosPorGimnasioCasoDeUso = obtenerTodosPorGimnasioCasoDeUso;
            _obtenerTodasLasRutinasPorProfesorCasoDeUso = obtenerTodasLasRutinasPorProfesorCasoDeUso;
            _obtenerEstadisticasProfesoresCasoDeUso = obtenerEstadisticasProfesoresCasoDeUso;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodosAsync()
        {
            try
            {
                var profesores = await _obtenerTodosLosProfesoresCasoDeUso.Ejecutar();
                return Ok(profesores);
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
                var profesor = await _obtenerProfesorPorIdCasoDeUso.Ejecutar(id);
                if (profesor == null)
                    return NotFound(new { Mensaje = $"No se encontró ningún profesor con ID {id}." });

                return Ok(profesor);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgregarAsync([FromBody] AgregarProfesorDTO profesorDTO)
        {
            if (profesorDTO == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var nuevoProfesor = await _agregarProfesorCasoDeUso.Ejecutar(profesorDTO);
                return Ok(nuevoProfesor);
            }
            catch (Exception ex)
            {
                if (ex.Message == "EMAIL_DUPLICADO")
                    return BadRequest(new { Mensaje = "Ya existe un profesor con este email." });

                if (ex.Message == "DNI_DUPLICADO")
                    return BadRequest(new { Mensaje = "Ya existe un profesor con este DNI." });

                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarProfesorDTO dto)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Id != id)
                return BadRequest(new { Mensaje = "El ID de la URL no coincide con el ID del profesor." });

            try
            {
                var actualizado = await _actualizarProfesorCasoDeUso.Ejecutar(id, dto);

                if (actualizado == null)
                    return NotFound(new { Mensaje = "Profesor no encontrado." });

                return Ok(actualizado);
            }
            catch (Exception ex)
            {
                if (ex.Message == "EMAIL_DUPLICADO")
                    return BadRequest(new { Mensaje = "Ya existe un profesor con este email." });

                if (ex.Message == "DNI_DUPLICADO")
                    return BadRequest(new { Mensaje = "Ya existe un profesor con este DNI." });

                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarAsync(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                var eliminado = await _eliminarProfesorCasoDeUso.Ejecutar(id);
                if (!eliminado)
                    return NotFound(new { Mensaje = $"No se encontró ningún profesor con ID {id}." });

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("gimnasio/{gimnasioId}")]
        public async Task<IActionResult> ObtenerPorGimnasio(long gimnasioId)
        {
            if (gimnasioId <= 0)
                return BadRequest(new { Mensaje = "El ID del gimnasio debe ser mayor a cero." });

            try
            {
                var profesores = await _obtenerTodosPorGimnasioCasoDeUso.Ejecutar(gimnasioId);
                return Ok(profesores);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("profesor/{usuarioId}")]
        public async Task<IActionResult> ObtenerRutinasPorProfesor(long usuarioId)
        {
            if (usuarioId <= 0)
                return BadRequest(new { Mensaje = "El ID del usuario debe ser mayor a cero." });

            try
            {
                var rutinas = await _obtenerTodasLasRutinasPorProfesorCasoDeUso.Ejecutar(usuarioId);

                if (rutinas == null || !rutinas.Any())
                    return NotFound(new { Mensaje = "No se encontraron rutinas para este profesor." });

                return Ok(rutinas);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("estadisticas")]
        public async Task<IActionResult> ObtenerEstadisticas()
        {
            try
            {
                var resultado = await _obtenerEstadisticasProfesoresCasoDeUso.Ejecutar();

                if (resultado == null)
                    return NotFound(new { Mensaje = "No hay datos de estadísticas de profesores." });

                return Ok(resultado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
