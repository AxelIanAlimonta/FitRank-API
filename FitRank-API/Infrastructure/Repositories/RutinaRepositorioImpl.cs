using System.Diagnostics.CodeAnalysis;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Application.DTOs.SesionDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories;

public class RutinaRepositorioImpl : IRutinaRepositorio
{
    private readonly FitRankDbContext _context;
    public RutinaRepositorioImpl(FitRankDbContext context)
    {
        _context = context;
    }

    public async Task<List<Rutina>> ObtenerTodasAsync()
    {
        return await _context.Rutinas
                        .Include(r => r.Usuario)
                        .Include(r => r.Socio)

                        .ToListAsync();
    }

    public async Task<Rutina?> ObtenerPorIdAsync(long id)
    {
        return await _context.Rutinas
                      .Include(r => r.Usuario)
                      .Include(r => r.Socio)
                      .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Rutina> AgregarAsync(Rutina rutina)
    {
        _context.Rutinas.Add(rutina);
        await _context.SaveChangesAsync();
        return rutina;
    }

    public async Task<Rutina?> ActualizarAsync(Rutina rutina)
    {
        var rutinaExistente = await _context.Rutinas.FindAsync(rutina.Id);
        if (rutinaExistente == null)
        {
            return null;
        }
        rutinaExistente.Nombre = rutina.Nombre ?? rutinaExistente.Nombre;
        rutinaExistente.TipoCreacion = rutina.TipoCreacion ?? rutinaExistente.TipoCreacion;
        rutinaExistente.Descripcion = rutina.Descripcion ?? rutinaExistente.Descripcion;
        rutinaExistente.Activa = rutina.Activa;
        rutinaExistente.SocioId = rutina.SocioId;

        await _context.SaveChangesAsync();
        return rutinaExistente;
    }

    public async Task<bool> EliminarAsync(long id)
    {
        var rutinaExistente = await _context.Rutinas.FindAsync(id);
        if (rutinaExistente == null)
        {
            return false;
        }
        _context.Rutinas.Remove(rutinaExistente);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<List<Rutina>> ObtenerPorSocioIdAsync(long socioId)
    {
        return _context.Rutinas
            .Where(r => r.SocioId == socioId)
            .ToListAsync();
    }

    [ExcludeFromCodeCoverage]
    public async Task<ResultadoConfirmarRutinaDTO> ValidarReferenciasAsync(ConfirmarRutinaDTO body)
    {
        var socioExiste = await _context.Set<Socio>().AnyAsync(s => s.Id == body.SocioId);
        if (!socioExiste)
            return ResultadoConfirmarRutinaDTO.Fallo($"SocioId {body.SocioId} no existe.");

        var ejercicioIds = body.Rutina.SesionesPlan
            .SelectMany(s => s.Ejercicios.Select(e => (long)e.EjercicioId))
            .Distinct()
            .ToList();

        var existentes = await _context.Ejercicios
            .Where(x => ejercicioIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync();

        var faltantes = ejercicioIds.Except(existentes).ToList();
        if (faltantes.Count > 0)
            return ResultadoConfirmarRutinaDTO.Fallo("Hay ejercicios inexistentes.");

        return ResultadoConfirmarRutinaDTO.Exito(0);
    }

    [ExcludeFromCodeCoverage]
    public async Task GuardarRutinaCompletaAsync(Rutina rutina, List<SesionIADTO> sesiones)
    {
        await using var trx = await _context.Database.BeginTransactionAsync();

        _context.Add(rutina);
        await _context.SaveChangesAsync();

        for (int i = 0; i < sesiones.Count; i++)
        {
            var s = sesiones[i];

            var sesion = new Sesion
            {
                RutinaId = rutina.Id,
                Nombre = s.Nombre,
                NumeroDeSesion = i,
                EjerciciosAsignados = new List<EjercicioAsignado>()
            };

            _context.Add(sesion);
            await _context.SaveChangesAsync();

            int orden = 1;
            foreach (var e in s.Ejercicios)
            {
                var ejAsig = new EjercicioAsignado
                {
                    SesionId = sesion.Id,
                    EjercicioId = e.EjercicioId,
                    NumeroEjercicio = orden++,
                    Series = e.Series.Select(sr => new Serie
                    {
                        NumeroDeSerie = sr.Nro,
                        Repeticiones = sr.Reps,
                        Peso = sr.PesoObjetivo
                    }).ToList()
                };

                _context.Add(ejAsig);
                await _context.SaveChangesAsync();
            }
        }

        await trx.CommitAsync();
    }

    public async Task<List<Rutina>> ObtenerRutinasPorSocioAsync(long socioId)
    {
        return await _context.Rutinas
            .Where(r => r.SocioId == socioId)
            .Include(r => r.Sesiones!)
                .ThenInclude(s => s.EjerciciosAsignados!)
                    .ThenInclude(ea => ea.Series!)
            .Include(r => r.Sesiones!)
                .ThenInclude(s => s.EjerciciosAsignados!)
                    .ThenInclude(ea => ea.Ejercicio!)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Rutina>> ObtenerTodasLasRutinasPorProfesorIdAsync(long profesorUsuarioId)
    {
        return await _context.Rutinas
            .Include(r => r.Socio)
            .Include(r => r.Usuario)
            .Where(r => r.UsuarioId == profesorUsuarioId && r.Usuario.Rol == "Profesor")
            .OrderByDescending(r => r.FechaCreacion)
            .ToListAsync();

    }

    public async Task<List<Rutina>> ObtenerFavoritasPorSocioAsync(long socioId)
    {
        return await _context.Rutinas
            .Where(r => r.SocioId == socioId && r.Favorita)
            .Include(r => r.Sesiones!)
                .ThenInclude(s => s.EjerciciosAsignados!)
                    .ThenInclude(ea => ea.Series!)
            .Include(r => r.Sesiones!)
                .ThenInclude(s => s.EjerciciosAsignados!)
                    .ThenInclude(ea => ea.Ejercicio!)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> MarcarFavoritaAsync(long rutinaId, bool favorita)
    {
        var rutina = await _context.Rutinas.FindAsync(rutinaId);
        if (rutina == null)
        {
            return false;
        }
        rutina.Favorita = favorita;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CambiarEstadoRutinaAsync(long rutinaId, bool activa)
    {
        var rutina = await _context.Rutinas.FindAsync(rutinaId);
        if (rutina == null)
            return false;

        rutina.Activa = activa;
        await _context.SaveChangesAsync();
        return true;
    }
}
