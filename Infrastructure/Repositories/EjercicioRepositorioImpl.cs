using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories;

public class EjercicioRepositorioImpl : IEjercicioRepositorio
{
    private readonly FitRankDbContext _context;
    public EjercicioRepositorioImpl(FitRankDbContext context)
    {
        _context = context;
    }

    public async Task<List<Ejercicio>> GetAllAsync()
    {
        return await _context.Ejercicios.ToListAsync();
    }

    public async Task<Ejercicio?> AddAsync(Ejercicio ejercicio)
    {
        var result = await _context.Ejercicios.AddAsync(ejercicio);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task DeleteAsync(long id)
    {
        var ejercicio = await _context.Ejercicios.FindAsync(id);
        if (ejercicio != null)
        {
            _context.Ejercicios.Remove(ejercicio);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(Ejercicio ejercicio)
    {
        var original = await _context.Ejercicios.FindAsync(ejercicio.Id);
        if (original == null)
            throw new KeyNotFoundException("Ejercicio not found");

        // Actualiza solo las propiedades necesarias
        _context.Entry(original).CurrentValues.SetValues(ejercicio);

        await _context.SaveChangesAsync();
    }


    public async Task<Ejercicio?> GetByIdAsync(long id)
    {
        return await _context.Ejercicios.FindAsync(id);
    }


}
