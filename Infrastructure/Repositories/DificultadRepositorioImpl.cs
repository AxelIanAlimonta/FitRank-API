using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories;

public class DificultadRepositorioImpl : IDificultadRepositorio
{
    private readonly FitRankDbContext _context;

    public DificultadRepositorioImpl(FitRankDbContext context)
    {
        _context = context;
    }

    public async Task<List<Dificultad>> ObtenerTodosAsync()
    {
        return await _context.Dificultades.ToListAsync();
    }
    public async Task<Dificultad?> ObtenerPorIdAsync(long id)
    {
        return await _context.Dificultades.FindAsync(id);
    }
    public async Task<Dificultad?> AgregarAsync(Dificultad dificultad)
    {
        var resultado = await _context.Dificultades.AddAsync(dificultad);
        await _context.SaveChangesAsync();
        return resultado.Entity;
    }

    public async Task<Dificultad?> ActualizarAsync(Dificultad dificultad)
    {
        var existente = await _context.Dificultades.FindAsync(dificultad.Id);
        if (existente == null)
        {
            return null;
        }
        existente.Descripcion = dificultad.Descripcion;
        await _context.SaveChangesAsync();
        return existente;
    }

    public async Task EliminarAsync(long id)
    {
        var dificultad = await _context.Dificultades.FindAsync(id);
        if (dificultad != null)
        {
            _context.Dificultades.Remove(dificultad);
            await _context.SaveChangesAsync();
        }
    }
}
