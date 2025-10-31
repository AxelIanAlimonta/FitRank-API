using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories;

public class AsistenciaRepositorioImpl : IAsistenciaRepositorio
{
    private readonly FitRankDbContext _context;

    public AsistenciaRepositorioImpl(FitRankDbContext context)
    {
        _context = context;
    }


    public async Task<Asistencia> AgregarAsync(Asistencia asistencia)
    {
        _context.Asistencias.Add(asistencia);
        await _context.SaveChangesAsync();
        return asistencia;
    }


    public async Task<List<Asistencia>> ObtenerPorUsuarioAsync(long usuarioId)
    {
        return await _context.Asistencias
            .Include(a => a.Gimnasio)
            .Where(a => a.UsuarioId == usuarioId)
            .OrderByDescending(a => a.Fecha)
            .ToListAsync();
    }



    public async Task<Asistencia?> ObtenerPorIdAsync(long id)
    {
        return await _context.Asistencias
            .Include(a => a.Gimnasio)
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Asistencia?> ActualizarAsync(Asistencia asistencia)
    {
        var existingAsistencia = await _context.Asistencias.FindAsync(asistencia.Id);
        if (existingAsistencia == null)
        {
            return null;
        }

        _context.Entry(existingAsistencia).CurrentValues.SetValues(asistencia);
        await _context.SaveChangesAsync();
        return existingAsistencia;

    }

    public async Task<IEnumerable<Asistencia>> ObtenerTodasAsync()
    {
        return await _context.Asistencias
            .Include(a => a.Usuario)
            .Include(a => a.Gimnasio)
            .OrderByDescending(a => a.Fecha)
            .ToListAsync();
    }


    public async Task<List<Asistencia>> ObtenerPorGimnasioYRangoAsync(long gimnasioId, DateTime? desde = null, DateTime? hasta = null)
    {
        var query = _context.Asistencias
            .Where(a => a.GimnasioId == gimnasioId);

        if (desde.HasValue)
            query = query.Where(a => a.Fecha >= desde.Value);

        if (hasta.HasValue)
            query = query.Where(a => a.Fecha <= hasta.Value);

        return await query.ToListAsync();
    }

    public async Task<bool> EliminarAsync(long Id)
    {
        var asistencia = await _context.Asistencias.FindAsync(Id);
        if (asistencia == null)
            return false;

        _context.Asistencias.Remove(asistencia);
        await _context.SaveChangesAsync();
        return true;
    }

}
