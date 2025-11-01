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


    public RutinaController(
        AgregarRutinaCasoDeUso agregarRutinaCasoDeUso,
        ObtenerRutinaPorIdCasoDeUso obtenerRutinaPorIdCasoDeUso,
        ActualizarRutinaCasoDeUso actualizarRutinaCasoDeUso,
        ObtenerTodasLasRutinasCasoDeUso obtenerTodasLasRutinasCasoDeUso,
        EliminarRutinaCasoDeUso eliminarRutinaCasoDeUso)
    {
        _agregarRutinaCasoDeUso = agregarRutinaCasoDeUso;
        _obtenerRutinaPorIdCasoDeUso = obtenerRutinaPorIdCasoDeUso;
        _obtenerTodasLasRutinasCasoDeUso = obtenerTodasLasRutinasCasoDeUso;
        _actualizarRutinaCasoDeUso = actualizarRutinaCasoDeUso;
        _eliminarRutinaCasoDeUso = eliminarRutinaCasoDeUso;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodo()
    {
        try
        {
            var rutinas = await _obtenerTodasLasRutinasCasoDeUso.Ejecutar();
            return Ok(rutinas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    [Route("{id:long}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        try
        {
            var rutina = await _obtenerRutinaPorIdCasoDeUso.Ejecutar(id);

            if (rutina == null)
            {
                return NotFound($"La rutina con ID {id} no fue encontrada.");
            }

            return Ok(rutina);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }




    //post
    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] AgregarRutinaDTO rutinaDTO)
    {
        if (rutinaDTO == null)
        {
            return BadRequest("El objeto rutina no puede ser nulo.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var rutinaCreada = await _agregarRutinaCasoDeUso.Ejecutar(rutinaDTO);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = rutinaCreada.Id }, rutinaCreada);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarRutinaDTO rutinaDTO)
    {
        if (rutinaDTO == null)
        {
            return BadRequest("El objeto rutina no puede ser nulo.");
        }

        if (id != rutinaDTO.Id)
        {
            return BadRequest("El ID de la ruta no coincide con el ID del objeto rutina.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var rutinaActualizada = await _actualizarRutinaCasoDeUso.Ejecutar(rutinaDTO);

            if (rutinaActualizada == null)
            {
                return NotFound($"La rutina con ID {id} no fue encontrada.");
            }

            return Ok(rutinaActualizada);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }

    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> Eliminar(long id)
    {
        try
        {
            var resultado = await _eliminarRutinaCasoDeUso.Ejecutar(id);

            if (!resultado)
            {
                return NotFound($"La rutina con ID {id} no fue encontrada.");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al eliminar rutina");
        }
    }

}
