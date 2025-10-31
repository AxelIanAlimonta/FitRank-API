using AutoMapper;
using FitRank.API.Application.Rutinas.Abstractions;
using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
    public async Task<IActionResult> Generar(
            [FromBody] RutinaRequestDTO input,
            [FromServices] IRoutineRulesRunner rulesRunner,
            [FromServices] IRoutineBuilder builder)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var decisiones = await rulesRunner.RunAsync(input);

        // Si las reglas piden derivación, avisamos (puede ser 409 o 200 con flag)
        if (decisiones.DerivarProfesional)
        {
            var explain = new { message = "Se requiere derivación/validación profesional", decisiones };
            return StatusCode(409, new { ok = false, explain });
        }

        var rutina = await builder.BuildAsync(input, decisiones);

        return Ok(new
        {
            ok = true,
            decisions = decisiones,
            rutina
        });
    }

    [HttpPost("confirmar")]
    public async Task<IActionResult> Confirmar(
    [FromBody] ConfirmarRutinaDTO body,
    [FromServices] FitRankDbContext db)
    {
        if (body is null || body.Rutina is null)
            return BadRequest("Body vacío.");

        // 1) Validaciones de FK básicas
        var socioExiste = await db.Set<Socio>().AnyAsync(s => s.Id == body.SocioId);

        if (!socioExiste) return BadRequest($"SocioId {body.SocioId} no existe.");

        // Validar que todos los ejercicios existen
        var ejercicioIds = body.Rutina.SesionesPlan
            .SelectMany(s => s.Ejercicios.Select(e => (long)e.EjercicioId))
            .Distinct()
            .ToList();

        var existentes = await db.Ejercicios
            .Where(x => ejercicioIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync();

        var faltantes = ejercicioIds.Except(existentes).ToList();
        if (faltantes.Count > 0)
            return BadRequest(new { error = "Hay ejercicios inexistentes", faltantes });

        // 2) Persistir en transacción
        await using var trx = await db.Database.BeginTransactionAsync();

        // Serializo a JsonDocument para Npgsql (se mapea a jsonb)
        JsonDocument? snapDoc = null;
        JsonDocument? rulesDoc = null;
        try
        {
            if (body.Rutina.InputSnapshot is not null)
                snapDoc = JsonDocument.Parse(JsonSerializer.Serialize(body.Rutina.InputSnapshot));
            if (body.Rutina.RulesExplain is not null)
                rulesDoc = JsonDocument.Parse(JsonSerializer.Serialize(body.Rutina.RulesExplain));
        }
        catch
        {
            // Si algo raro en el JSON, seguí sin romper: dejá null o guardá string si preferís
        }

        var rutina = new Rutina
        {
            Nombre = body.Rutina.Nombre,
            TipoCreacion = "IA",                      // <- importante para diferenciar origen
            FechaCreacion = DateTime.UtcNow,
            Descripcion = $"{body.Rutina.Objetivo} · {body.Rutina.Division}",
            Activa = true,                            // o false si querés activar sólo tras aprobación
            SocioId = body.SocioId,
            InputSnapshotJson = snapDoc,
            RulesExplainJson = rulesDoc,
            Sesiones = new List<Sesion>()
        };

        db.Add(rutina);
        await db.SaveChangesAsync();

        // Sesiones + ejercicios + series
        for (int i = 0; i < body.Rutina.SesionesPlan.Count; i++)
        {
            var s = body.Rutina.SesionesPlan[i];

            var sesion = new Sesion
            {
                RutinaId = rutina.Id,
                Nombre = s.Nombre,
                NumeroDeSesion= i,
                EjerciciosAsignados = new List<EjercicioAsignado>()
            };
            db.Add(sesion);
            await db.SaveChangesAsync();

            int orden = 1;
            foreach (var e in s.Ejercicios)
            {
                var ejAsig = new EjercicioAsignado
                {
                    SesionId = sesion.Id,
                    EjercicioId = e.EjercicioId,
                    NumeroEjercicio = orden++,
                    Series = new List<Serie>()
                };
                db.Add(ejAsig);
                await db.SaveChangesAsync();

                foreach (var sr in e.Series)
                {
                    ejAsig.Series.Add(new Serie
                    {
                        EjercicioAsignadoId = ejAsig.Id,
                        NumeroDeSerie = sr.Nro,
                        Repeticiones = sr.Reps,
                        Peso = sr.PesoObjetivo.HasValue ? (int)Math.Round(sr.PesoObjetivo.Value) : 0
                    });
                }

                await db.SaveChangesAsync();
            }
        }

        await trx.CommitAsync();

        return Ok(new { ok = true, id = rutina.Id });
    }



}
