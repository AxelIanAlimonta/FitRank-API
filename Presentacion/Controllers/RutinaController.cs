using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.DTOs.RutinaDTOs;
using Microsoft.AspNetCore.Mvc;


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

    private readonly GenerarRutinaIACasoDeUso _generarRutinaIACasoDeUso;
    private readonly ConfirmarRutinaIACasoDeUso _confirmarRutinaIACasoDeUso;


    public RutinaController(
        AgregarRutinaCasoDeUso agregarRutinaCasoDeUso,
        ObtenerRutinaPorIdCasoDeUso obtenerRutinaPorIdCasoDeUso,
        ActualizarRutinaCasoDeUso actualizarRutinaCasoDeUso,
        ObtenerTodasLasRutinasCasoDeUso obtenerTodasLasRutinasCasoDeUso,
        EliminarRutinaCasoDeUso eliminarRutinaCasoDeUso,
        GenerarRutinaIACasoDeUso generarRutinaIACasoDeUso,
        ConfirmarRutinaIACasoDeUso confirmarRutinaIACasoDeUso)
    {
        _agregarRutinaCasoDeUso = agregarRutinaCasoDeUso;
        _obtenerRutinaPorIdCasoDeUso = obtenerRutinaPorIdCasoDeUso;
        _obtenerTodasLasRutinasCasoDeUso = obtenerTodasLasRutinasCasoDeUso;
        _actualizarRutinaCasoDeUso = actualizarRutinaCasoDeUso;
        _eliminarRutinaCasoDeUso = eliminarRutinaCasoDeUso;
        _generarRutinaIACasoDeUso = generarRutinaIACasoDeUso;
        _confirmarRutinaIACasoDeUso = confirmarRutinaIACasoDeUso;
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

    [HttpPost("generar")]
    public async Task<IActionResult> Generar([FromBody] RutinaRequestDTO input)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var resultado = await _generarRutinaIACasoDeUso.EjecutarAsync(input);

        if (resultado.RequiereDerivacion)
            return StatusCode(409, new { ok = false, explain = resultado.Mensaje });

        return Ok(new
        {
            ok = true,
            decisions = resultado.Decisiones,
            rutina = resultado.Rutina
        });
    }

    [HttpPost("confirmar")]
    public async Task<IActionResult> Confirmar([FromBody] ConfirmarRutinaDTO body)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var resultado = await _confirmarRutinaIACasoDeUso.EjecutarAsync(body);

        if (!resultado.Ok)
            return BadRequest(resultado.Mensaje);

        return Ok(new { ok = true, id = resultado.RutinaId });
    }
}
