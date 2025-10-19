using FitRank_API.Application.CasosDeUso.EjercicioCasosDeUso;
using FitRank_API.Application.DTOs.EjercicioDTOs;
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
        var ejercicios = await _obtenerEjerciciosCasoDeUso.EjecutarAsync();
        return Ok(ejercicios);
    }

    //get id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEjercicioPorId(long id)
    {
        var ejercicio = await _obtenerEjercicioPorIdCasoDeUso.EjecutarAsync(id);
        if (ejercicio == null)
        {
            return NotFound();
        }
        return Ok(ejercicio);
    }

    [HttpPost]
    public async Task<IActionResult> AgregarEjercicio([FromBody] AgregarEjercicioDTO ejercicio)
    {
        var nuevoEjercicio = await _agregarEjercicioCasoDeUso.EjecutarAsync(ejercicio);
        return CreatedAtAction(nameof(GetEjercicioPorId), new { id = nuevoEjercicio.Id }, nuevoEjercicio);

    }

    [HttpPut]
    public async Task<IActionResult> ActualizarEjercicio([FromBody] EjercicioDTO ejercicio)
    {
        var ejercicioActualizado = await _actualizarEjercicioCasoDeUso.EjecutarAsync(ejercicio);
        if (ejercicioActualizado == null)
        {
            return NotFound();
        }
        return Ok(ejercicioActualizado);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarEjercicio(long id)
    {
        var resultado = await _eliminarEjercicioCasoDeUso.EjecutarAsync(id);
        if (!resultado)
        {
            return NotFound();
        }
        return NoContent();
    }

}
