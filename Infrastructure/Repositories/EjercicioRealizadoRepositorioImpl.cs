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

    public async Task<List<EjercicioRealizado>> ObtenerTodosAsync()
    {
       return await _context.EjerciciosRealizados.Include(e => e.Ejercicio)
                                                 .Include(e => e.Socio)
                                                 .Include(e => e.Rutina)
                                                 .ToListAsync();
    }

    public async Task<EjercicioRealizado?> ObtenerPorIdAsync(long id)
    {
        return await _context.EjerciciosRealizados.Include(e => e.Ejercicio)
                                                 .Include(e => e.Socio)
                                                 .Include(e => e.Rutina)
                                                 .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<EjercicioRealizado> AgregarAsync(EjercicioRealizado ejercicioRealizado)
    {
        _context.EjerciciosRealizados.Add(ejercicioRealizado);
        await _context.SaveChangesAsync();
        return ejercicioRealizado;
    }

    public async Task<EjercicioRealizado?> ActualizarAsync(EjercicioRealizado ejercicioRealizado)
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

    public async Task<bool> EliminarAsync(long id)
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
