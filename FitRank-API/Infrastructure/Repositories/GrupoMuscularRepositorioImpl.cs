using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories;

public class GrupoMuscularRepositorioImpl : IGrupoMuscularRepositorio
{

    private readonly FitRankDbContext _context;

    public GrupoMuscularRepositorioImpl(FitRankDbContext context)
    {
        _context = context;
    }

    public async Task<List<GrupoMuscular>> ObtenerTodosAsync()
    {
        return await _context.GruposMusculares.ToListAsync();
    }

    public async Task<GrupoMuscular?> ObtenerPorIdAsync(long id)
    {
        return await _context.GruposMusculares.FirstOrDefaultAsync(g => g.Id == id);

    }

    public async Task<GrupoMuscular?> AgregarAsync(GrupoMuscular grupoMuscular)
    {
        var resultado = await _context.GruposMusculares.AddAsync(grupoMuscular);
        await _context.SaveChangesAsync();
        return resultado.Entity;
    }

    public async Task<GrupoMuscular?> ActualizarAsync(GrupoMuscular grupoMuscular)
    {
        var existente = await _context.GruposMusculares.FindAsync(grupoMuscular.Id);
        if (existente == null)
        {
            return null;
        }

        existente.Nombre = grupoMuscular.Nombre;
        existente.Imagen = grupoMuscular.Imagen;

        await _context.SaveChangesAsync();
        return existente;
    }


    public async Task<bool> EliminarAsync(long id)
    {
        var grupoMuscular = await _context.GruposMusculares.FindAsync(id);
        if (grupoMuscular == null) return false;
        
            _context.GruposMusculares.Remove(grupoMuscular);
            await _context.SaveChangesAsync();
            return true;
        
    }
}
