using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories;

public class SerieAsignadaRepositorioImpl : ISerieAsignadaRepositorio
{
    private readonly FitRankDbContext _context;
    public SerieAsignadaRepositorioImpl(FitRankDbContext context)
    {
        _context = context;
    }

    public async Task<SerieAsignada> AgregarAsync(SerieAsignada serieAsignada)
    {
        _context.SeriesAsignadas.Add(serieAsignada);
        await _context.SaveChangesAsync();
        return serieAsignada;
    }

    public async Task<bool> EliminarAsync(long id)
    {
        var serieAsignada = await _context.SeriesAsignadas.FindAsync(id);
        if (serieAsignada == null)
        {
            return false;
        }
        _context.SeriesAsignadas.Remove(serieAsignada);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<SerieAsignada>> ObtenerTodasAsync()
    {

        return await _context.SeriesAsignadas.Include(s => s.EjercicioAsignado).ToListAsync();
    }

    public async Task<SerieAsignada?> ObtenerPorIdAsync(long id)
    {
        return await _context.SeriesAsignadas
            .Include(s => s.EjercicioAsignado)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<SerieAsignada?> ActualizarAsync(SerieAsignada serieAsignada)
    {
        var existente = await _context.SeriesAsignadas.FindAsync(serieAsignada.Id);
        if (existente == null)
        {
            return null;
        }
        existente.Peso = serieAsignada.Peso;
        existente.Repeticiones = serieAsignada.Repeticiones;
        existente.Rir = serieAsignada.Rir;
        existente.NroSerie = serieAsignada.NroSerie;
        existente.EjercicioAsignadoId = serieAsignada.EjercicioAsignadoId;
        await _context.SaveChangesAsync();
        return existente;
    }


}
