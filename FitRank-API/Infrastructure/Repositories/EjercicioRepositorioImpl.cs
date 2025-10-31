using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace FitRank_API.Infrastructure.Repositories;

public class EjercicioRepositorioImpl : IEjercicioRepositorio
{
    private readonly FitRankDbContext _context;
    public EjercicioRepositorioImpl(FitRankDbContext context)
    {
        _context = context;
    }

    public async Task<List<Ejercicio>> ObtenerEjerciciosAsync()
    {
        return await _context.Ejercicios
                .Include(e => e.GrupoMuscular)
                .Include(e => e.Maquina)
                .ToListAsync();
    }

    public async Task<Ejercicio?> ObtenerEjercicioPorIdAsync(long id)
    {
        return await _context.Ejercicios
            .Include(e => e.GrupoMuscular)
            .Include(e => e.Maquina)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
    public async Task<Ejercicio> AgregarEjercicioAsync(Ejercicio ejercicio)
    {
        _context.Ejercicios.Add(ejercicio);
        await _context.SaveChangesAsync();

        // Recargar con relaciones
        var ejercicioConRelaciones = await _context.Ejercicios
            .Include(e => e.GrupoMuscular)
            .Include(e => e.Maquina)
            .FirstOrDefaultAsync(e => e.Id == ejercicio.Id);

        return ejercicioConRelaciones!;
    }

    public async Task<Ejercicio?> ActualizarEjercicioAsync(Ejercicio ejercicio)
    {
        var ejercicioExistente = await _context.Ejercicios.FindAsync(ejercicio.Id);
        if (ejercicioExistente == null)
        {
            return null;
        }
        _context.Entry(ejercicioExistente).CurrentValues.SetValues(ejercicio);
        await _context.SaveChangesAsync();

        var ejercicioConRelaciones = await _context.Ejercicios
            .Include(e => e.GrupoMuscular)
            .Include(e => e.Maquina)
            .FirstOrDefaultAsync(e => e.Id == ejercicioExistente.Id);
        return ejercicioConRelaciones;
    }

    public async Task<bool> EliminarEjercicioAsync(long id)
    {
        var ejercicioExistente = await _context.Ejercicios.FindAsync(id);
        if (ejercicioExistente == null)
        {
            return false;
        }
        _context.Ejercicios.Remove(ejercicioExistente);
        await _context.SaveChangesAsync();
        return true;
    }
}
