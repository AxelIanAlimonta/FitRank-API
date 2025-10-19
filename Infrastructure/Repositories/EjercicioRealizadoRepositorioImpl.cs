using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories;

public class EjercicioRealizadoRepositorioImpl : IEjercicioRealizadoRepositorio
{
    private readonly FitRankDbContext _context;
    public EjercicioRealizadoRepositorioImpl(FitRankDbContext context)
    {
        _context = context;
    }

    public async Task<List<EjercicioRealizado>> ObtenerTodos()
    {
        return await _context.EjerciciosRealizados.Include(er => er.SeriesRealizadas)
            .Include(er => er.Ejercicio)       
            .Include(er => er.Socio)            
            .Include(er => er.Rutina)
            .Include(er => er.SesionRealizadaDeEjercicios)
            .ToListAsync();
    }

    public async Task<EjercicioRealizado?> ObtenerPorId(long id)
    {
        return await _context.EjerciciosRealizados.Include(er => er.SeriesRealizadas)
            .Include(er => er.Ejercicio)
            .Include(er => er.Socio)
            .Include(er => er.Rutina)
            .Include(er => er.SesionRealizadaDeEjercicios)
            .FirstOrDefaultAsync(er => er.Id == id);
    }

    public async Task<EjercicioRealizado> Agregar(EjercicioRealizado ejercicioRealizado)
    {
        _context.EjerciciosRealizados.Add(ejercicioRealizado);
        await _context.SaveChangesAsync();
        return ejercicioRealizado;
    }

    public async Task<EjercicioRealizado?> Actualizar(EjercicioRealizado ejercicioRealizado)
    {
        var existingEjercicioRealizado = await _context.EjerciciosRealizados.FindAsync(ejercicioRealizado.Id);
        if (existingEjercicioRealizado == null)
        {
            return null;
        }
        _context.Entry(existingEjercicioRealizado).CurrentValues.SetValues(ejercicioRealizado);
        await _context.SaveChangesAsync();
        return existingEjercicioRealizado;
    }

    public async Task<bool> Eliminar(long id)
    {
        var ejercicioRealizado = await _context.EjerciciosRealizados.FindAsync(id);
        if (ejercicioRealizado == null)
        {
            return false;
        }
        _context.EjerciciosRealizados.Remove(ejercicioRealizado);
        await _context.SaveChangesAsync();
        return true;
    }
}
