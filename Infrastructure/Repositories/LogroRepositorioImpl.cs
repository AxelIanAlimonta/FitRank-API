using FitRank_API.Application.DTOs.LogroDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories;

public class LogroRepositorioImpl : ILogroRepositorio
{
    private readonly FitRankDbContext _context;
    public LogroRepositorioImpl(FitRankDbContext context)
    {
        _context = context;
    }

    public async Task<List<Logro>> ObtenerTodosLosLogros()
    {
        return await _context.Logros.ToListAsync();
    }

    public async Task<Logro?> ObtenerLogroPorId(long id)
    {
        return await _context.Logros.FindAsync(id);
    }

    public async Task<Logro> AgregarLogro(Logro logro)
    {
        _context.Logros.Add(logro);
        await _context.SaveChangesAsync();
        return logro;
    }

    public async Task<Logro?> ActualizarLogro(long id, ActualizarLogroDTO dto)
    {
        var logroExistente = await _context.Logros.FirstOrDefaultAsync(x => x.Id == id);
        if (logroExistente is null)
            return null;

        if (!string.IsNullOrWhiteSpace(dto.Nombre)) logroExistente.Nombre = dto.Nombre;
        if (!string.IsNullOrWhiteSpace(dto.Descripcion)) logroExistente.Descripcion = dto.Descripcion;
        if (!string.IsNullOrWhiteSpace(dto.Categoria)) logroExistente.Categoria = dto.Categoria;
        if (!string.IsNullOrWhiteSpace(dto.Imagen)) logroExistente.Imagen = dto.Imagen;
        if (dto.Puntos.HasValue) logroExistente.Puntos = dto.Puntos.Value;

        await _context.SaveChangesAsync();
        return logroExistente;
    }



    public async Task<bool> EliminarLogro(long id)
    {
        var logro = await _context.Logros.FindAsync(id);
        if (logro == null)
        {
            return false;
        }
        _context.Logros.Remove(logro);
        await _context.SaveChangesAsync();
        return true;
    }
}
