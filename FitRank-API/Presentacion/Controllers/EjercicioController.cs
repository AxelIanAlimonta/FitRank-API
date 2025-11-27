using FitRank_API.Application.CasosDeUso.EjercicioCasosDeUso;
using FitRank_API.Application.DTOs.EjercicioDTOs.ActualizarEjercicioDTO;
using FitRank_API.Application.DTOs.EjercicioDTOs.AgregarEjercicioDTO;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EjercicioController : ControllerBase
{
    private readonly ObtenerEjerciciosCasoDeUso _obtenerEjerciciosCasoDeUso;
    private readonly ObtenerEjercicioPorIdCasoDeUso _obtenerEjercicioPorIdCasoDeUso;
    private readonly AgregarEjercicioCasoDeUso _agregarEjercicioCasoDeUso;
    private readonly ActualizarEjercicioCasoDeUso _actualizarEjercicioCasoDeUso;
    private readonly EliminarEjercicioCasoDeUso _eliminarEjercicioCasoDeUso;
    private readonly ObtenerEjerciciosPorGrupoMuscularCasoDeUso _obtenerEjerciciosporGrupoMuscularCasoDeUso;

    public EjercicioController(
        ObtenerEjerciciosCasoDeUso obtenerEjerciciosCasoDeUso,
        ObtenerEjercicioPorIdCasoDeUso obtenerEjercicioPorIdCasoDeUso,
        AgregarEjercicioCasoDeUso agregarEjercicioCasoDeUso,
        ActualizarEjercicioCasoDeUso actualizarEjercicioCasoDeUso,
        EliminarEjercicioCasoDeUso eliminarEjercicioCasoDeUso,
        ObtenerEjerciciosPorGrupoMuscularCasoDeUso obtenerEjerciciosporGrupoMuscularCasoDeUso)
    {
        _obtenerEjerciciosCasoDeUso = obtenerEjerciciosCasoDeUso;
        _obtenerEjercicioPorIdCasoDeUso = obtenerEjercicioPorIdCasoDeUso;
        _agregarEjercicioCasoDeUso = agregarEjercicioCasoDeUso;
        _actualizarEjercicioCasoDeUso = actualizarEjercicioCasoDeUso;
        _eliminarEjercicioCasoDeUso = eliminarEjercicioCasoDeUso;
        _obtenerEjerciciosporGrupoMuscularCasoDeUso = obtenerEjerciciosporGrupoMuscularCasoDeUso;
    }

    [HttpGet]
    public async Task<IActionResult> GetEjercicios()
    {
        try
        {
            var ejercicios = await _obtenerEjerciciosCasoDeUso.Ejecutar();
            return Ok(ejercicios);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEjercicioPorId(long id)
    {
        if (id <= 0)
            return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

        try
        {
            var ejercicio = await _obtenerEjercicioPorIdCasoDeUso.Ejecutar(id);
            if (ejercicio == null)
            {
                return NotFound(new { Mensaje = "Ejercicio no encontrado." });
            }
            return Ok(ejercicio);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AgregarEjercicio([FromBody] AgregarEjercicioDTO ejercicio)
    {
        if (ejercicio == null)
        {
            return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var nuevoEjercicio = await _agregarEjercicioCasoDeUso.Ejecutar(ejercicio);
            return CreatedAtAction(nameof(GetEjercicioPorId), new { id = nuevoEjercicio.Id }, nuevoEjercicio);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarEjercicio(long id, [FromBody] ActualizarEjercicioDTO ejercicio)
    {
        if (id <= 0)
            return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

        if (ejercicio == null)
        {
            return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != ejercicio.Id)
        {
            return BadRequest(new { Mensaje = "El ID de la URL no coincide con el ID del ejercicio." });
        }

        try
        {
            var ejercicioActualizado = await _actualizarEjercicioCasoDeUso.Ejecutar(ejercicio);
            if (ejercicioActualizado == null)
            {
                return NotFound(new { Mensaje = "Ejercicio no encontrado." });
            }
            return Ok(ejercicioActualizado);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarEjercicio(long id)
    {
        if (id <= 0)
            return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

        try
        {
            var resultado = await _eliminarEjercicioCasoDeUso.Ejecutar(id);
            if (!resultado)
            {
                return NotFound(new { Mensaje = "Ejercicio no encontrado." });
            }
            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpGet("grupo/{grupoMuscularId}")]
    public async Task<IActionResult> GetEjerciciosPorGrupoMuscular(long grupoMuscularId)
    {
        if (grupoMuscularId <= 0)
            return BadRequest(new { Mensaje = "El ID del grupo muscular debe ser mayor a cero." });

        try
        {
            var ejercicios = await _obtenerEjerciciosporGrupoMuscularCasoDeUso.Ejecutar(grupoMuscularId);
            return Ok(ejercicios);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }
}
