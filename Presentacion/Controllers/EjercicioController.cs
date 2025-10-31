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
        var ejercicios = await _obtenerEjerciciosCasoDeUso.EjecutarAsync();
        return Ok(ejercicios);
    }

    //get id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEjercicioPorId(long id)
    {
        var ejercicio = await _obtenerEjercicioPorIdCasoDeUso.Ejecutar(id);
        if (ejercicio == null)
        {
            return NotFound();
        }
        return Ok(ejercicio);
    }

    [HttpPost]
    public async Task<IActionResult> AgregarEjercicio([FromBody] AgregarEjercicioDTO ejercicio)
    {
        var nuevoEjercicio = await _agregarEjercicioCasoDeUso.Ejecutar(ejercicio);
        return CreatedAtAction(nameof(GetEjercicioPorId), new { id = nuevoEjercicio.Id }, nuevoEjercicio);

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarEjercicio(long id, [FromBody] ActualizarEjercicioDTO ejercicio)
    {
        if (id != ejercicio.Id) return BadRequest("El ID no coincide.");
        var ejercicioActualizado = await _actualizarEjercicioCasoDeUso.Ejecutar(ejercicio);
        if (ejercicioActualizado == null)
        {
            return NotFound();
        }
        return Ok(ejercicioActualizado);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarEjercicio(long id)
    {
        var resultado = await _eliminarEjercicioCasoDeUso.Ejecutar(id);
        if (!resultado)
        {
            return NotFound();
        }
        return NoContent();
    }

    //[HttpGet("grupo/{grupoFuncionalId}")]
    //public async Task<IActionResult> GetEjerciciosPorGrupoFuncional(long grupoFuncionalId)
    //{
    //    var ejercicios = await _obtenerEjerciciosPorGrupoFuncionalCasoDeUso.EjecutarAsync(grupoFuncionalId);
    //    return Ok(ejercicios);
    //}


}
