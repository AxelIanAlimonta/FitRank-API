using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
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

    public async Task<IEnumerable<Socio>> ObtenerTodosAsync()
    {
        return await _context.Socios.ToListAsync();
    }

    // ObtenerSocioPorIdAsync
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

        // Actualiza las propiedades necesarias
        socioExistente.Nombre = socio.Nombre;
        socioExistente.Apellido = socio.Apellido;

        await _context.SaveChangesAsync();
        return socioExistente;
    }

    //eliminar socio
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


}
