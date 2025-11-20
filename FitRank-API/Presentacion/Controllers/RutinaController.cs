using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.DTOs.RutinaDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


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
    private readonly ObtenerRutinaCompletaCasoDeUso _obtenerRutinaCompletaCasoDeUso;
    private readonly ObtenerRutinasFavoritasCasoDeUso _obtenerFavoritasCasoDeUso;
    private readonly MarcarDesmarcarRutinaFavoritaCasoDeUso _marcarDesmarcarRutinaFavoritaCasoDeUso;
    private readonly CambiarEstadoRutinaCasoDeUso _cambiarEstadoRutinaCasoDeUso;




    public RutinaController(
        AgregarRutinaCasoDeUso agregarRutinaCasoDeUso,
        ObtenerRutinaPorIdCasoDeUso obtenerRutinaPorIdCasoDeUso,
        ActualizarRutinaCasoDeUso actualizarRutinaCasoDeUso,
        ObtenerTodasLasRutinasCasoDeUso obtenerTodasLasRutinasCasoDeUso,
        EliminarRutinaCasoDeUso eliminarRutinaCasoDeUso,
        GenerarRutinaIACasoDeUso generarRutinaIACasoDeUso,
        ConfirmarRutinaIACasoDeUso confirmarRutinaIACasoDeUso,
        ObtenerRutinaCompletaCasoDeUso obtenerRutinaCompletaCasoDeUso,
        MarcarDesmarcarRutinaFavoritaCasoDeUso marcarDesmarcarRutinaFavoritaCasoDeUso,
        CambiarEstadoRutinaCasoDeUso cambiarEstadoRutinaCasoDeUso,
        ObtenerRutinasFavoritasCasoDeUso obtenerRutinasFavoritasCasoDeUso)
    {
        _agregarRutinaCasoDeUso = agregarRutinaCasoDeUso;
        _obtenerRutinaPorIdCasoDeUso = obtenerRutinaPorIdCasoDeUso;
        _actualizarRutinaCasoDeUso = actualizarRutinaCasoDeUso;
        
        _obtenerTodasLasRutinasCasoDeUso = obtenerTodasLasRutinasCasoDeUso;
        _eliminarRutinaCasoDeUso = eliminarRutinaCasoDeUso;
        _generarRutinaIACasoDeUso = generarRutinaIACasoDeUso;
        _confirmarRutinaIACasoDeUso = confirmarRutinaIACasoDeUso;
        _obtenerRutinaCompletaCasoDeUso = obtenerRutinaCompletaCasoDeUso;
        _obtenerFavoritasCasoDeUso = obtenerRutinasFavoritasCasoDeUso;
        _marcarDesmarcarRutinaFavoritaCasoDeUso = marcarDesmarcarRutinaFavoritaCasoDeUso;
        _cambiarEstadoRutinaCasoDeUso = cambiarEstadoRutinaCasoDeUso;
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

    [HttpGet("socio/{socioId}/detalle")]
    public async Task<IActionResult> ObtenerRutinaCompletaPorSocio(long socioId)
    {
        var resultado = await _obtenerRutinaCompletaCasoDeUso.Ejecutar(socioId);

        if (resultado == null || !resultado.Any())
            return NotFound("No se encontraron rutinas para este socio.");

        return Ok(resultado);
    }

    [HttpPost("generar")]
    //[Authorize(Roles = "Socio")]
    public async Task<IActionResult> Generar(long idSocio,[FromBody] RutinaRequestDTO input)
    {

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var resultado = await _generarRutinaIACasoDeUso.EjecutarAsync(input);

        if (resultado.RequiereDerivacion)
            return StatusCode(409, new { ok = false, explain = resultado.Mensaje, decisions = resultado.Decisiones });

        ConfirmarRutinaDTO confirmarBody = new ConfirmarRutinaDTO(idSocio, idSocio, resultado.Rutina);

        var rutina = await _confirmarRutinaIACasoDeUso.EjecutarAsync(confirmarBody);
        

        return Ok(new
        {
            ok = true,
            decisions = resultado.Decisiones,
            rutina = resultado.Rutina,
            id = rutina.RutinaId
        });
    }

    [HttpPut("rutina/{rutinaId}/favorita")]
    public async Task<IActionResult> CambiarFavorita(long rutinaId, [FromQuery] bool favorita)
    {
        var ok = await _marcarDesmarcarRutinaFavoritaCasoDeUso.Ejecutar(rutinaId, favorita);
        if (!ok)
            return NotFound("Rutina no encontrada");

        return Ok(new { mensaje = "Rutina actualizada", favorita });
    }

    [HttpPut("rutina/{rutinaId}/estado")]
    public async Task<IActionResult> CambiarEstado(long rutinaId, [FromQuery] bool activa)
    {
        var ok = await _cambiarEstadoRutinaCasoDeUso.Ejecutar(rutinaId, activa);
        if (!ok)
            return NotFound("Rutina no encontrada");

        return Ok(new { mensaje = "Rutina actualizada", activa });
    }

    [HttpGet("rutina/favoritas/{socioId}")]
    public async Task<IActionResult> GetFavoritas(long socioId)
    {
        var list = await _obtenerFavoritasCasoDeUso.Ejecutar(socioId);
        return Ok(list);
    }

    /*
    [HttpPost("confirmar")]
    public async Task<IActionResult> Confirmar([FromBody] ConfirmarRutinaDTO body)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var resultado = await _confirmarRutinaIACasoDeUso.EjecutarAsync(body);

        if (!resultado.Ok)
            return BadRequest(resultado.Mensaje);

        return Ok(new { ok = true, id = resultado.RutinaId });
    }*/
}
