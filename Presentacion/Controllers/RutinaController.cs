using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;


namespace FitRank_API.Presentacion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RutinaController : ControllerBase
{

    private readonly AgregarRutinaCasoDeUso _agregarRutinaCasoDeUso;
    private readonly ObtenerRutinaPorIdCasoDeUso _obtenerRutinaPorIdCasoDeUso;
    private readonly ActualizarRutinaCasoDeUso _actualizarRutinaCasoDeUso;
    private readonly EliminarRutinaCasoDeUso _eliminarRutinaCasoDeUso;
    private readonly ObtenerTodasLasRutinasCasoDeUso _obtenerTodasLasRutinasCasoDeUso;
    private readonly ObtenerRutinaCompletaCasoDeUso _obtenerRutinaCompletaCasoDeUso;



    public RutinaController(
          AgregarRutinaCasoDeUso agregarRutinaCasoDeUso,
          ObtenerRutinaPorIdCasoDeUso obtenerRutinaPorIdCasoDeUso,
          ActualizarRutinaCasoDeUso actualizarRutinaCasoDeUso,
          EliminarRutinaCasoDeUso eliminarRutinaCasoDeUso,
          ObtenerTodasLasRutinasCasoDeUso obtenerTodasLasRutinasCasoDeUso,
          ObtenerRutinaCompletaCasoDeUso obtenerRutinaCompletaCasoDeUso
          )
    {
        _agregarRutinaCasoDeUso = agregarRutinaCasoDeUso;
        _obtenerRutinaPorIdCasoDeUso = obtenerRutinaPorIdCasoDeUso;
        _actualizarRutinaCasoDeUso = actualizarRutinaCasoDeUso;
        _eliminarRutinaCasoDeUso = eliminarRutinaCasoDeUso;
        _obtenerTodasLasRutinasCasoDeUso = obtenerTodasLasRutinasCasoDeUso;
        _obtenerRutinaCompletaCasoDeUso = obtenerRutinaCompletaCasoDeUso;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodo()
    {
        var rutinas = await _obtenerTodasLasRutinasCasoDeUso.Ejecutar();
        return Ok(rutinas);
    }

    [HttpGet]
    [Route("{id:long}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var rutina = await _obtenerRutinaPorIdCasoDeUso.Ejecutar(id);
        if (rutina == null)
        {
            return NotFound();
        }
        return Ok(rutina);
    }

  

    //post
    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] AgregarRutinaDTO rutinaDTO)
    {
        var nuevaRutina = await _agregarRutinaCasoDeUso.Ejecutar(rutinaDTO);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaRutina.Id }, nuevaRutina);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(long id,[FromBody] ActualizarRutinaDTO rutinaDTO)
    {
        if (id != rutinaDTO.Id)
        {
            return BadRequest();
        }
        var rutinaActualizada = await _actualizarRutinaCasoDeUso.Ejecutar(rutinaDTO);
        if (rutinaActualizada == null)
        {
            return NotFound();
        }
        return Ok(rutinaActualizada);
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var eliminado = await _eliminarRutinaCasoDeUso.Ejecutar(id);
        if (!eliminado)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("socio/{socioId}/detalle")]
    public async Task<IActionResult> ObtenerRutinaCompletaPorSocio(long socioId)
    {
        var resultado = await _obtenerRutinaCompletaCasoDeUso.Ejecutar(socioId);

        if (resultado == null || !resultado.Any())
            return NotFound("No se encontraron rutinas para este socio.");

        return Ok(resultado);
    }

}
