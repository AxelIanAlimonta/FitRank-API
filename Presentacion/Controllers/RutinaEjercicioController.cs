using FitRank_API.Application.CasosDeUso.RutinaEjerciciosCasosDeUso;
using FitRank_API.Application.DTOs.RutinaEjercicioDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RutinaEjercicioController : ControllerBase
{
    private readonly ObtenerRutinaEjercicioPorIdCasoDeUso _obtenerRutinaEjercicioPorIdCasoDeUso;
    private readonly ObtenerTodasRutinasEjerciciosCasoDeUso _obtenerTodasRutinasEjerciciosCasoDeUso;
    private readonly ActualizarRutinaEjercicioCasoDeUso _actualizarRutinaEjercicioCasoDeUso;
    private readonly EliminarRutinaEjercicioCasoDeUso _eliminarRutinaEjercicioCasoDeUso;
    private readonly AgregarRutinaEjercicioCasoDeUso _crearRutinaEjercicioCasoDeUso;

    public RutinaEjercicioController(
        ObtenerRutinaEjercicioPorIdCasoDeUso obtenerRutinaEjercicioPorIdCasoDeUso,
        ObtenerTodasRutinasEjerciciosCasoDeUso obtenerTodasRutinasEjerciciosCasoDeUso,
        ActualizarRutinaEjercicioCasoDeUso actualizarRutinaEjercicioCasoDeUso,
        EliminarRutinaEjercicioCasoDeUso eliminarRutinaEjercicioCasoDeUso,
        AgregarRutinaEjercicioCasoDeUso crearRutinaEjercicioCasoDeUso)
    {
        _obtenerRutinaEjercicioPorIdCasoDeUso = obtenerRutinaEjercicioPorIdCasoDeUso;
        _obtenerTodasRutinasEjerciciosCasoDeUso = obtenerTodasRutinasEjerciciosCasoDeUso;
        _actualizarRutinaEjercicioCasoDeUso = actualizarRutinaEjercicioCasoDeUso;
        _eliminarRutinaEjercicioCasoDeUso = eliminarRutinaEjercicioCasoDeUso;
        _crearRutinaEjercicioCasoDeUso = crearRutinaEjercicioCasoDeUso;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var rutinasEjercicios = await _obtenerTodasRutinasEjerciciosCasoDeUso.Ejecutar();
        return Ok(rutinasEjercicios);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var rutinaEjercicio = await _obtenerRutinaEjercicioPorIdCasoDeUso.Ejecutar(id);
        if (rutinaEjercicio == null)
        {
            return NotFound();
        }
        return Ok(rutinaEjercicio);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] AgregarRutinaEjercicioDTO agregarRutinaEjercicioDTO)
    {
        var rutinaEjercicioCreado = await _crearRutinaEjercicioCasoDeUso.Ejecutar(agregarRutinaEjercicioDTO);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = rutinaEjercicioCreado.Id }, rutinaEjercicioCreado);

    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarRutinaEjercicioDTO actualizarRutinaEjercicioDTO)
    {
        if (id != actualizarRutinaEjercicioDTO.Id)
        {
            return BadRequest();
        }
        var rutinaEjercicioActualizado = await _actualizarRutinaEjercicioCasoDeUso.Ejecutar(actualizarRutinaEjercicioDTO);
        if (rutinaEjercicioActualizado == null)
        {
            return NotFound();
        }
        return Ok(rutinaEjercicioActualizado);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var eliminado = await _eliminarRutinaEjercicioCasoDeUso.Ejecutar(id);
        if (!eliminado)
        {
            return NotFound();
        }
        return NoContent();
    }

}
