using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories;

public class EjercicioAsignadoRepositorioImpl : IEjercicioAsignadoRepositorio
{
    private readonly FitRankDbContext _context;

    public EjercicioAsignadoRepositorioImpl(FitRankDbContext context)
    {
        _context = context;
    }

    public async Task<EjercicioAsignado> AgregarAsync(EjercicioAsignado ejercicioAsignado)
    {
        var resultado = await _context.EjerciciosAsignados.AddAsync(ejercicioAsignado);
        await _context.SaveChangesAsync();

        var ejercicioConDetalles = await _context.EjerciciosAsignados
            .Include(e => e.Ejercicio)
                .ThenInclude(e => e.GrupoMuscular)
            .Include(e => e.Ejercicio)
                .ThenInclude(e => e.Maquina)
            .Include(e => e.Sesion)
            .FirstOrDefaultAsync(e => e.Id == resultado.Entity.Id);
        return ejercicioConDetalles;
    }

    public async Task<bool> EliminarAsync(long id)
    {
        var ejercicioAsignado = await _context.EjerciciosAsignados.FindAsync(id);
        if (ejercicioAsignado == null)
        {
            return false;
        }
        _context.EjerciciosAsignados.Remove(ejercicioAsignado);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<EjercicioAsignado>> ObtenerTodosAsync()
    {
        return await _context.EjerciciosAsignados
            .Include(e => e.Ejercicio)
                .ThenInclude(e => e.Maquina)
            .Include(e => e.Ejercicio)
                .ThenInclude(e => e.GrupoMuscular)
            .Include(e => e.Sesion)
            .ToListAsync();
    }

    public async Task<EjercicioAsignado?> ObtenerPorIdAsync(long id)
    {
        return await _context.EjerciciosAsignados
            .Include(e => e.Ejercicio)
                .ThenInclude(e => e.Maquina)
            .Include(e => e.Ejercicio)
                .ThenInclude(e => e.GrupoMuscular)
            .Include(e => e.Sesion)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<EjercicioAsignado?> ActualizarAsync(EjercicioAsignado ejercicioAsignado)
    {
        var ejercicioExistente = await _context.EjerciciosAsignados
        .Include(ea => ea.Ejercicio)
            .ThenInclude(e => e.GrupoMuscular)
        .Include(ea => ea.Ejercicio)
            .ThenInclude(e => e.Maquina)
         .Include(ea => ea.Sesion)
        .FirstOrDefaultAsync(ea => ea.Id == ejercicioAsignado.Id);

        if (ejercicioExistente == null)
        {
            return null;
        }

        ejercicioExistente.NumeroEjercicio = ejercicioAsignado.NumeroEjercicio;
        ejercicioExistente.EjercicioId = ejercicioAsignado.EjercicioId;
        ejercicioExistente.SesionId = ejercicioAsignado.SesionId;

        await _context.SaveChangesAsync();

        return ejercicioExistente;
    }
}
