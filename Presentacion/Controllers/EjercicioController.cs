using FitRank_API.Application.CasosDeUso.EjercicioCasosDeUso;
using FitRank_API.Application.DTOs.EjercicioDTOs;
using FitRank_API.Application.DTOs.EjercicioDTOs.ActualizarEjercicioDTO;
using FitRank_API.Application.DTOs.EjercicioDTOs.AgregarEjercicioDTO;
using Microsoft.AspNetCore.Http;
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

    public EjercicioController(
        ObtenerEjerciciosCasoDeUso obtenerEjerciciosCasoDeUso,
        ObtenerEjercicioPorIdCasoDeUso obtenerEjercicioPorIdCasoDeUso,
        AgregarEjercicioCasoDeUso agregarEjercicioCasoDeUso,
        ActualizarEjercicioCasoDeUso actualizarEjercicioCasoDeUso,
        EliminarEjercicioCasoDeUso eliminarEjercicioCasoDeUso)
    {
        _obtenerEjerciciosCasoDeUso = obtenerEjerciciosCasoDeUso;
        _obtenerEjercicioPorIdCasoDeUso = obtenerEjercicioPorIdCasoDeUso;
        _agregarEjercicioCasoDeUso = agregarEjercicioCasoDeUso;
        _actualizarEjercicioCasoDeUso = actualizarEjercicioCasoDeUso;
        _eliminarEjercicioCasoDeUso = eliminarEjercicioCasoDeUso;
    }

    [HttpGet]
    public async Task<IActionResult> GetEjercicios()
    {
        try
        {
            var ejercicios = await _obtenerEjerciciosCasoDeUso.EjecutarAsync();
            return Ok(ejercicios);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    //get id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEjercicioPorId(long id)
    {
        var ejercicio = await _obtenerEjercicioPorIdCasoDeUso.Ejecutar(id);
        if (ejercicio == null)
        {
            return NotFound($"El ejercicio con ID {id} no fue encontrado.");
        }
        return Ok(ejercicio);
    }

    [HttpPost]
    public async Task<IActionResult> AgregarEjercicio([FromBody] AgregarEjercicioDTO ejercicio)
    {
        if (ejercicio == null)
        {
            return BadRequest("El ejercicio no puede ser nulo.");
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
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarEjercicio(long id, [FromBody] ActualizarEjercicioDTO ejercicio)
    {
        if (ejercicio == null)
        {
            return BadRequest("El ejercicio no puede ser nulo.");
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (id != ejercicio.Id) return BadRequest("El ID del ejercicio no coincide con el ID proporcionado en la ruta.");
        try
        {
            var ejercicioActualizado = await _actualizarEjercicioCasoDeUso.Ejecutar(ejercicio);
            if (ejercicioActualizado == null)
            {
                return NotFound($"El ejercicio con ID {id} no fue encontrado para actualizar.");
            }
            return Ok(ejercicioActualizado);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error al actualizar ejercicio");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarEjercicio(long id)
    {
        try
        {
            var resultado = await _eliminarEjercicioCasoDeUso.Ejecutar(id);
            if (!resultado)
            {
                return NotFound($"El ejercicio con ID {id} no fue encontrado para eliminar.");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error al eliminar ejercicio");
        }
    }

    //[HttpGet("grupo/{grupoFuncionalId}")]
    //public async Task<IActionResult> GetEjerciciosPorGrupoFuncional(long grupoFuncionalId)
    //{
    //    var ejercicios = await _obtenerEjerciciosPorGrupoFuncionalCasoDeUso.EjecutarAsync(grupoFuncionalId);
    //    return Ok(ejercicios);
    //}


}
