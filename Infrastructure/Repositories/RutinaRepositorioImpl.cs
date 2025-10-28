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

    public Task<Rutina> ObtenerPorSocioIdAsync(long socioId)
    {
        throw new NotImplementedException();
    }
}
