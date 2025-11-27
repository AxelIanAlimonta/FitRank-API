using FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.GrupoMuscularDTOs;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GrupoMuscularController : ControllerBase
{
    private readonly ObtenerTodosLosGruposMuscularesCasoDeUso _obtenerTodosLosGruposMuscularesCasoDeUso;
    private readonly ObtenerGrupoMuscularPorIdCasoDeUso _obtenerGrupoMuscularPorIdCasoDeUso;
    private readonly AgregarGrupoMuscularCasoDeUso _agregarGrupoMuscularCasoDeUso;
    private readonly ActualizarGrupoMuscularCasoDeUso _actualizarGrupoMuscularCasoDeUso;
    private readonly EliminarGrupoMuscularCasoDeUso _eliminarGrupoMuscularCasoDeUso;

    public GrupoMuscularController(
        ObtenerTodosLosGruposMuscularesCasoDeUso obtenerTodosLosGruposMuscularesCasoDeUso,
        ObtenerGrupoMuscularPorIdCasoDeUso obtenerGrupoMuscularPorIdCasoDeUso,
        EliminarGrupoMuscularCasoDeUso eliminarGrupoMuscularCasoDeUso,
        ActualizarGrupoMuscularCasoDeUso actualizarGrupoMuscularCasoDeUso,
        AgregarGrupoMuscularCasoDeUso agregarGrupoMuscularCasoDeUso)
    {
        _obtenerTodosLosGruposMuscularesCasoDeUso = obtenerTodosLosGruposMuscularesCasoDeUso;
        _obtenerGrupoMuscularPorIdCasoDeUso = obtenerGrupoMuscularPorIdCasoDeUso;
        _eliminarGrupoMuscularCasoDeUso = eliminarGrupoMuscularCasoDeUso;
        _actualizarGrupoMuscularCasoDeUso = actualizarGrupoMuscularCasoDeUso;
        _agregarGrupoMuscularCasoDeUso = agregarGrupoMuscularCasoDeUso;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        try
        {
            var gruposMusculares = await _obtenerTodosLosGruposMuscularesCasoDeUso.Ejecutar();
            return Ok(gruposMusculares);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        if (id <= 0)
            return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

        try
        {
            var grupoMuscular = await _obtenerGrupoMuscularPorIdCasoDeUso.Ejecutar(id);
            if (grupoMuscular == null)
            {
                return NotFound(new { Mensaje = "Grupo muscular no encontrado." });
            }
            return Ok(grupoMuscular);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] AgregarGrupoMuscularDTO grupoMuscular)
    {
        if (grupoMuscular == null)
        {
            return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var nuevoGrupoMuscular = await _agregarGrupoMuscularCasoDeUso.Ejecutar(grupoMuscular);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoGrupoMuscular.Id }, nuevoGrupoMuscular);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarGrupoMuscularDTO grupoMuscular)
    {
        if (id <= 0)
            return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

        if (grupoMuscular == null)
        {
            return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });
        }

        if (id != grupoMuscular.Id)
        {
            return BadRequest(new { Mensaje = "El ID de la URL no coincide con el ID del grupo muscular." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var grupoMuscularActualizado = await _actualizarGrupoMuscularCasoDeUso.Ejecutar(grupoMuscular);
            if (grupoMuscularActualizado == null)
            {
                return NotFound(new { Mensaje = "Grupo muscular no encontrado." });
            }
            return Ok(grupoMuscularActualizado);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Eliminar(long id)
    {
        if (id <= 0)
            return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

        try
        {
            var eliminado = await _eliminarGrupoMuscularCasoDeUso.Ejecutar(id);
            if (eliminado)
                return NoContent();
            else
                return NotFound(new { Mensaje = "Grupo muscular no encontrado." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }
}
