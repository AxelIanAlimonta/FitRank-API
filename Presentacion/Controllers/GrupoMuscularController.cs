using FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;
using FitRank_API.Application.DTOs.GrupoMuscularDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GrupoMuscularController : ControllerBase
{
    private readonly ObtenerTodosLosGruposMuscularesCasoDeUso obtenerTodosLosGruposMuscularesCasoDeUso;
    private readonly ObtenerGrupoMuscularPorIdCasoDeUso obtenerGrupoMuscularPorIdCasoDeUso;
    private readonly AgregarGrupoMuscularCasoDeUso agregarGrupoMuscularCasoDeUso;
    private readonly ActualizarGrupoMuscularCasoDeUso actualizarGrupoMuscularCasoDeUso;
    private readonly EliminarGrupoMuscularCasoDeUso eliminarGrupoMuscularCasoDeUso;

    public GrupoMuscularController(ObtenerTodosLosGruposMuscularesCasoDeUso obtenerTodosLosGruposMuscularesCasoDeUso,
            ObtenerGrupoMuscularPorIdCasoDeUso obtenerGrupoMuscularPorIdCasoDeUso,
            EliminarGrupoMuscularCasoDeUso eliminarGrupoMuscularCasoDeUso,
            ActualizarGrupoMuscularCasoDeUso actualizarGrupoMuscularCasoDeUso,
            AgregarGrupoMuscularCasoDeUso agregarGrupoMuscularCasoDeUso)
    {
        this.obtenerTodosLosGruposMuscularesCasoDeUso = obtenerTodosLosGruposMuscularesCasoDeUso;
        this.obtenerGrupoMuscularPorIdCasoDeUso = obtenerGrupoMuscularPorIdCasoDeUso;
        this.eliminarGrupoMuscularCasoDeUso = eliminarGrupoMuscularCasoDeUso;
        this.actualizarGrupoMuscularCasoDeUso = actualizarGrupoMuscularCasoDeUso;
        this.agregarGrupoMuscularCasoDeUso = agregarGrupoMuscularCasoDeUso;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var gruposMusculares = await obtenerTodosLosGruposMuscularesCasoDeUso.Ejecutar();
        return Ok(gruposMusculares);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var grupoMuscular = await obtenerGrupoMuscularPorIdCasoDeUso.Ejecutar(id);
        if (grupoMuscular == null)
        {
            return NotFound();
        }
        return Ok(grupoMuscular);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] AgregarGrupoMuscularDTO grupoMuscular)
    {
        var nuevoGrupoMuscular = await agregarGrupoMuscularCasoDeUso.Ejecutar(grupoMuscular);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoGrupoMuscular.Id }, nuevoGrupoMuscular);
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] GrupoMuscularDTO grupoMuscular)
    {
        if (id != grupoMuscular.Id)
        {
            return BadRequest("El ID del grupo muscular no coincide.");
        }
        var grupoMuscularActualizado = await actualizarGrupoMuscularCasoDeUso.Ejecutar(grupoMuscular);
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
        await eliminarGrupoMuscularCasoDeUso.Ejecutar(id);
        return NoContent();
    }
}
