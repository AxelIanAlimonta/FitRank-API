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

    public async Task<List<Rutina>> ObtenerTodas()
    {
        return await _context.Rutinas.Include(r => r.Dificultad).ToListAsync();
    }

    public async Task<Rutina?> ObtenerPorId(long id)
    {
        return await _context.Rutinas.Include(r => r.Dificultad).FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Rutina> Agregar(Rutina rutina)
    {
        _context.Rutinas.Add(rutina);
        await _context.SaveChangesAsync();
        return rutina;
    }

    public async Task<Rutina?> Actualizar(Rutina rutina)
    {
        var rutinaExistente = await _context.Rutinas.FindAsync(rutina.Id);
        if (rutinaExistente == null)
        {
            return null;
        }
        rutinaExistente.Nombre = rutina.Nombre;
        rutinaExistente.Frecuencia = rutina.Frecuencia;
        rutinaExistente.DificultadId = rutina.DificultadId;
        await _context.SaveChangesAsync();
        return rutinaExistente;
    }

    public async Task<bool> Eliminar(long id)
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


}
