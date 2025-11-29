using FitRank_API.Application.DTOs.PuntajeDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories;

public class SocioRepositorioImpl : ISocioRepositorio
{
    private readonly FitRankDbContext _context;
    public SocioRepositorioImpl(FitRankDbContext context)
    {
        _context = context;
    }

    public async Task<List<Socio>> ObtenerTodosAsync()
    {
        return await _context.Socios.ToListAsync();
    }

    
    public async Task<Socio?> ObtenerPorIdAsync(long id)
    {
        return await _context.Socios.FindAsync(id);
    }

    public async Task<Socio> AgregarAsync(Socio socio)
    {
        _context.Socios.Add(socio);
        await _context.SaveChangesAsync();
        return socio;
    }

    public async Task<Socio?> ActualizarAsync(Socio socio)
    {
        var socioExistente = await _context.Socios.FindAsync(socio.Id);
        if (socioExistente == null)
        {
            return null;
        }

        socioExistente.Nombre = socio.Nombre;
        socioExistente.Apellido = socio.Apellido;
        socioExistente.Email = socio.Email;
        socioExistente.Altura = socio.Altura;
        socioExistente.Peso = socio.Peso;
        socioExistente.Nivel = socio.Nivel;
        socioExistente.GimnasioId = socio.GimnasioId;
        socioExistente.ParticipaEnRanking = socio.ParticipaEnRanking;


        await _context.SaveChangesAsync();
        return socioExistente;
    }

    public async Task<bool> EliminarAsync(long id)
    {
        var socioExistente = await _context.Socios.FindAsync(id);
        if (socioExistente == null)
        {
            return false;
        }
        _context.Socios.Remove(socioExistente);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Socio?> ObtenerSocioConMedidasAsync(long socioId)
    {
        return await _context.Socios
            .Include(s => s.MedidasCorporales)
            .FirstOrDefaultAsync(s => s.Id == socioId);
    }

    public async Task<Socio?> ObtenerSocioConEntrenamientosAsync(long socioId)
    {
        return await _context.Socios
                    .Include(s => s.Entrenamientos)
                        .ThenInclude(e => e.Actividades)
                            .ThenInclude(a => a.Serie)
                    .Include(s => s.Entrenamientos)
                        .ThenInclude(e => e.Actividades)
                            .ThenInclude(a => a.EjercicioAsignado)
                                .ThenInclude(ea => ea.Ejercicio)
                                    .ThenInclude(ex => ex.GrupoMuscular)
                    .FirstOrDefaultAsync(s => s.Id == socioId);
    }

    public async Task<IEnumerable<Socio>> ObtenerTodosConEntrenamientoAsync()
    {
        return await _context.Usuarios
            .OfType<Socio>() 
            .Include(s => s.Entrenamientos)
                .ThenInclude(e => e.Actividades)
                    .ThenInclude(a => a.Serie)
                        .ThenInclude(s => s.EjercicioAsignado)
                            .ThenInclude(ea => ea.Ejercicio)
                                .ThenInclude(e => e.GrupoMuscular)
            .ToListAsync();
    }

    public async Task<IEnumerable<Socio>> ObtenerTodosPorGimnasio(long gimnasioId)
    {
        return await _context.Socios
            .Where(s => s.GimnasioId == gimnasioId)
            .ToListAsync();
    }

    public async Task<bool> CambiarParticipacionRankingAsync(long socioId, bool participa)
    {
        var socio = await _context.Socios.FindAsync(socioId);
        if (socio == null)
        {
            return false;
        }
        socio.ParticipaEnRanking = participa;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Socio>> ObtenerSociosParaRankingAsync(long gimnasioId)
    {
        return await _context.Usuarios
            .OfType<Socio>()
            .Where(s => s.GimnasioId == gimnasioId && s.ParticipaEnRanking)
            .Include(s => s.Entrenamientos)
                .ThenInclude(e => e.Actividades)
                    .ThenInclude(a => a.Serie)
                        .ThenInclude(s => s.EjercicioAsignado)
                            .ThenInclude(ea => ea.Ejercicio)
                                .ThenInclude(e => e.GrupoMuscular)
            .ToListAsync();
    }

    public async Task<List<SocioRankingDto>> ObtenerRankingGeneralAsync(long gimnasioId, int cantidad)
    {
        var query = await (
            from socio in _context.Usuarios.OfType<Socio>()
            join entrenamiento in _context.Entrenamientos on socio.Id equals entrenamiento.SocioId into entJoin
            from ent in entJoin.DefaultIfEmpty()
            join actividad in _context.Actividades on ent.Id equals actividad.EntrenamientoId into actJoin
            from act in actJoin.DefaultIfEmpty()
            where socio.GimnasioId == gimnasioId && socio.ParticipaEnRanking
            group act by new { socio.Id, socio.Nombre, socio.Apellido } into g
            select new SocioRankingDto
            {
                SocioId = g.Key.Id,
                NombreCompleto = g.Key.Nombre + " " + g.Key.Apellido,
                PuntajeTotal = g.Sum(a => a.Punto ?? 0)
            })
            .OrderByDescending(x => x.PuntajeTotal)
            .Take(cantidad)
            .ToListAsync();

        return query;
    }

    public async Task<Socio?> ObtenerSocioYUsuarioPorIdAsync(long socioId)
    {
        return await _context.Socios
            .Include(s => s.Gimnasio)
            .FirstOrDefaultAsync(s => s.Id == socioId);
    }

}
