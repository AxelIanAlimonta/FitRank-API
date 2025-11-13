using FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso;
using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.DTOs.ProfesorDTOs;
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

        public ProfesorController(
            AgregarProfesorCasoDeUso agregarProfesorCasoDeUso,
            ObtenerProfesorPorIdCasoDeUso obtenerProfesorPorIdCasoDeUso,
            ActualizarProfesorCasoDeUso actualizarProfesorCasoDeUso,
            ObtenerTodosLosProfesoresCasoDeUso obtenerTodosLosProfesoresCasoDeUso,
            EliminarProfesorCasoDeUso eliminarProfesorCasoDeUso, ObtenerTodasLasRutinasPorProfesorCasoDeUso obtenerTodasLasRutinasPorProfesorCasoDeUso)
        {
            _agregarProfesorCasoDeUso = agregarProfesorCasoDeUso;
            _obtenerProfesorPorIdCasoDeUso = obtenerProfesorPorIdCasoDeUso;
            _obtenerTodosLosProfesoresCasoDeUso = obtenerTodosLosProfesoresCasoDeUso;
            _actualizarProfesorCasoDeUso = actualizarProfesorCasoDeUso;
            _eliminarProfesorCasoDeUso = eliminarProfesorCasoDeUso;
            _obtenerTodasLasRutinasPorProfesorCasoDeUso = obtenerTodasLasRutinasPorProfesorCasoDeUso;
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
            var nuevoProfesor = await _agregarProfesorCasoDeUso.Ejecutar(profesorDTO);
            return Ok(nuevoProfesor);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarAsync(long id, [FromBody] ActualizarProfesorDTO profesorDTO)
        {
            // Validación: el id de la URL debe coincidir con el del cuerpo
            if (id != profesorDTO.Id)
                return BadRequest("El ID de la URL no coincide con el del cuerpo.");

            var profesorActualizado = await _actualizarProfesorCasoDeUso.Ejecutar(id, profesorDTO);

            if (profesorActualizado == null)
                return NotFound();

            return Ok(profesorActualizado);
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

    }
}
