using FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.GrupoMuscularDTOs;
using Microsoft.AspNetCore.Http;
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

    public GrupoMuscularController(ObtenerTodosLosGruposMuscularesCasoDeUso obtenerTodosLosGruposMuscularesCasoDeUso,
            ObtenerGrupoMuscularPorIdCasoDeUso obtenerGrupoMuscularPorIdCasoDeUso,
            EliminarGrupoMuscularCasoDeUso eliminarGrupoMuscularCasoDeUso,
            ActualizarGrupoMuscularCasoDeUso actualizarGrupoMuscularCasoDeUso,
            AgregarGrupoMuscularCasoDeUso agregarGrupoMuscularCasoDeUso)
    {
        this._obtenerTodosLosGruposMuscularesCasoDeUso = obtenerTodosLosGruposMuscularesCasoDeUso;
        this._obtenerGrupoMuscularPorIdCasoDeUso = obtenerGrupoMuscularPorIdCasoDeUso;
        this._eliminarGrupoMuscularCasoDeUso = eliminarGrupoMuscularCasoDeUso;
        this._actualizarGrupoMuscularCasoDeUso = actualizarGrupoMuscularCasoDeUso;
        this._agregarGrupoMuscularCasoDeUso = agregarGrupoMuscularCasoDeUso;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var gruposMusculares = await _obtenerTodosLosGruposMuscularesCasoDeUso.Ejecutar();
        return Ok(gruposMusculares);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var grupoMuscular = await _obtenerGrupoMuscularPorIdCasoDeUso.Ejecutar(id);
        if (grupoMuscular == null)
        {
            return NotFound();
        }
        return Ok(grupoMuscular);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] AgregarGrupoMuscularDTO grupoMuscular)
    {
        var nuevoGrupoMuscular = await _agregarGrupoMuscularCasoDeUso.Ejecutar(grupoMuscular);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoGrupoMuscular.Id }, nuevoGrupoMuscular);
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ObtenerGrupoMuscularDTO grupoMuscular)
    {
        if (id != grupoMuscular.Id)
        {
            return BadRequest("El ID del grupo muscular no coincide.");
        }
        var grupoMuscularActualizado = await _actualizarGrupoMuscularCasoDeUso.Ejecutar(grupoMuscular);
        if (grupoMuscularActualizado == null)
        {
            return NotFound();
        }
        return Ok(grupoMuscularActualizado);

    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var eliminado = await _eliminarGrupoMuscularCasoDeUso.Ejecutar(id);
        if (!eliminado)
        {
            return NotFound();
        }
        return NoContent();
    }
}
