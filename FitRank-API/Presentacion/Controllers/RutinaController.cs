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
    private readonly ObtenerRutinaCompletaCasoDeUso _obtenerRutinaCompletaCasoDeUso;
    private readonly ObtenerRutinasFavoritasCasoDeUso _obtenerFavoritasCasoDeUso;
    private readonly MarcarDesmarcarRutinaFavoritaCasoDeUso _marcarDesmarcarRutinaFavoritaCasoDeUso;
    private readonly CambiarEstadoRutinaCasoDeUso _cambiarEstadoRutinaCasoDeUso;

    public RutinaController(
        AgregarRutinaCasoDeUso agregarRutinaCasoDeUso,
        ObtenerRutinaPorIdCasoDeUso obtenerRutinaPorIdCasoDeUso,
        ActualizarRutinaCasoDeUso actualizarRutinaCasoDeUso,
        EliminarRutinaCasoDeUso eliminarRutinaCasoDeUso,
        ObtenerTodasLasRutinasCasoDeUso obtenerTodasLasRutinasCasoDeUso,
        GenerarRutinaIACasoDeUso generarRutinaIACasoDeUso,
        ConfirmarRutinaIACasoDeUso confirmarRutinaIACasoDeUso,
        ObtenerRutinaCompletaCasoDeUso obtenerRutinaCompletaCasoDeUso,
        ObtenerRutinasFavoritasCasoDeUso obtenerFavoritasCasoDeUso,
        MarcarDesmarcarRutinaFavoritaCasoDeUso marcarDesmarcarRutinaFavoritaCasoDeUso,
        CambiarEstadoRutinaCasoDeUso cambiarEstadoRutinaCasoDeUso)
    {
        _agregarRutinaCasoDeUso = agregarRutinaCasoDeUso;
        _obtenerRutinaPorIdCasoDeUso = obtenerRutinaPorIdCasoDeUso;
        _actualizarRutinaCasoDeUso = actualizarRutinaCasoDeUso;
        _eliminarRutinaCasoDeUso = eliminarRutinaCasoDeUso;
        _obtenerTodasLasRutinasCasoDeUso = obtenerTodasLasRutinasCasoDeUso;
        _generarRutinaIACasoDeUso = generarRutinaIACasoDeUso;
        _confirmarRutinaIACasoDeUso = confirmarRutinaIACasoDeUso;
        _obtenerRutinaCompletaCasoDeUso = obtenerRutinaCompletaCasoDeUso;
        _obtenerFavoritasCasoDeUso = obtenerFavoritasCasoDeUso;
        _marcarDesmarcarRutinaFavoritaCasoDeUso = marcarDesmarcarRutinaFavoritaCasoDeUso;
        _cambiarEstadoRutinaCasoDeUso = cambiarEstadoRutinaCasoDeUso;
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] AgregarRutinaDTO rutinaDTO)
    {
        if (rutinaDTO == null)
            return BadRequest(new { Mensaje = "El objeto rutina no puede ser nulo." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var rutinaCreada = await _agregarRutinaCasoDeUso.Ejecutar(rutinaDTO);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = rutinaCreada.Id }, rutinaCreada);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        if (id <= 0)
            return BadRequest(new { Mensaje = "El ID de la rutina debe ser mayor a cero." });

        try
        {
            var rutina = await _obtenerRutinaPorIdCasoDeUso.Ejecutar(id);

            if (rutina == null)
                return NotFound(new { Mensaje = $"La rutina con ID {id} no fue encontrada." });

            return Ok(rutina);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarRutinaDTO rutinaDTO)
    {
        if (id <= 0)
            return BadRequest(new { Mensaje = "El ID de la rutina debe ser mayor a cero." });

        if (rutinaDTO == null)
            return BadRequest(new { Mensaje = "El objeto rutina no puede ser nulo." });

        if (id != rutinaDTO.Id)
            return BadRequest(new { Mensaje = "El ID de la rutina no coincide con el ID del objeto rutina." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var rutinaActualizada = await _actualizarRutinaCasoDeUso.Ejecutar(rutinaDTO);

            if (rutinaActualizada == null)
                return NotFound(new { Mensaje = $"La rutina con ID {id} no fue encontrada." });

            return Ok(rutinaActualizada);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
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
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Eliminar(long id)
    {
        if (id <= 0)
            return BadRequest(new { Mensaje = "El ID de la rutina debe ser mayor a cero." });

        try
        {
            var resultado = await _eliminarRutinaCasoDeUso.Ejecutar(id);

            if (!resultado)
                return NotFound(new { Mensaje = $"La rutina con ID {id} no fue encontrada." });

            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpGet("socio/{socioId}/detalle")]
    public async Task<IActionResult> ObtenerRutinaCompletaPorSocio(long socioId)
    {
        if (socioId <= 0)
            return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

        try
        {
            var resultado = await _obtenerRutinaCompletaCasoDeUso.Ejecutar(socioId);

            if (resultado == null || !resultado.Any())
                return NotFound(new { Mensaje = "No se encontraron rutinas para este socio." });

            return Ok(resultado);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpPost("generar")]
    public async Task<IActionResult> Generar(long idSocio, [FromBody] RutinaRequestDTO input)
    {
        if (idSocio <= 0)
            return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

        if (input == null)
            return BadRequest(new { Mensaje = "Los datos de la rutina no pueden ser nulos." });

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
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
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpPut("rutina/{rutinaId}/favorita")]
    public async Task<IActionResult> CambiarFavorita(long rutinaId, [FromQuery] bool favorita)
    {
        if (rutinaId <= 0)
            return BadRequest(new { Mensaje = "El ID de la rutina debe ser mayor a cero." });

        try
        {
            var ok = await _marcarDesmarcarRutinaFavoritaCasoDeUso.Ejecutar(rutinaId, favorita);
            if (!ok)
                return NotFound(new { Mensaje = "Rutina no encontrada." });

            return Ok(new { mensaje = "Rutina actualizada", favorita });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpPut("rutina/{rutinaId}/estado")]
    public async Task<IActionResult> CambiarEstado(long rutinaId, [FromQuery] bool activa)
    {
        if (rutinaId <= 0)
            return BadRequest(new { Mensaje = "El ID de la rutina debe ser mayor a cero." });

        try
        {
            var ok = await _cambiarEstadoRutinaCasoDeUso.Ejecutar(rutinaId, activa);
            if (!ok)
                return NotFound(new { Mensaje = "Rutina no encontrada." });

            return Ok(new { mensaje = "Rutina actualizada", activa });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpGet("rutina/favoritas/{socioId}")]
    public async Task<IActionResult> GetFavoritas(long socioId)
    {
        if (socioId <= 0)
            return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

        try
        {
            var list = await _obtenerFavoritasCasoDeUso.Ejecutar(socioId);
            return Ok(list);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpGet("rutina/favoritas/gimnasio/{gimnasioId}")]
    public async Task<IActionResult> GetFavoritasGimnasio(long gimnasioId)
    {
        if (gimnasioId <= 0)
            return BadRequest(new { Mensaje = "El ID del gimnasio debe ser mayor a cero." });

        try
        {
            var list = await _obtenerFavoritasCasoDeUso.Ejecutar(gimnasioId);
            return Ok(list);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }
}
