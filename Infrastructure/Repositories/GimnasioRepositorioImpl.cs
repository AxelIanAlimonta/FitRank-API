using FitRank_API.Application.DTOs.GimnasioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories;

public class GimnasioRepositorioImpl : IGimnasioRepositorio
{
    private readonly FitRankDbContext _context;
    public GimnasioRepositorioImpl(FitRankDbContext context)
    {
        _context = context;
    }

    public async Task<List<Gimnasio>> ObtenerTodosLosGimnasios()
    {
        return await _context.Gimnasios.ToListAsync();
    }

    public async Task<Gimnasio?> ObtenerGimnasioPorId(long id)
    {
        return await _context.Gimnasios.FindAsync(id);
    }

    public async Task<Gimnasio> AgregarGimnasio(Gimnasio gimnasio)
    {
        _context.Gimnasios.Add(gimnasio);
        await _context.SaveChangesAsync();
        return gimnasio;
    }

    public async Task<Gimnasio?> ActualizarGimnasio(long id, ActualizarGimnasioDTO dto)
    {
        var gimnasioExistente = await _context.Gimnasios.FirstOrDefaultAsync(x => x.Id == id);
        if (gimnasioExistente is null)
            return null;

        if (!string.IsNullOrWhiteSpace(dto.Nombre)) gimnasioExistente.Nombre = dto.Nombre!;
        if (!string.IsNullOrWhiteSpace(dto.Direccion)) gimnasioExistente.Direccion = dto.Direccion!;
        if (!string.IsNullOrWhiteSpace(dto.RazonSocial)) gimnasioExistente.RazonSocial = dto.RazonSocial!;
        if (!string.IsNullOrWhiteSpace(dto.LogoUrl)) gimnasioExistente.LogoUrl = dto.LogoUrl!;
        if (!string.IsNullOrWhiteSpace(dto.ColorPrincipal)) gimnasioExistente.ColorPrincipal = dto.ColorPrincipal!; //VER SI NORMALIZAR O NO
        if (!string.IsNullOrWhiteSpace(dto.ColorSecundario)) gimnasioExistente.ColorSecundario = dto.ColorSecundario!; //VER SI NORMALIZAR O NO
        if (!string.IsNullOrWhiteSpace(dto.Email)) gimnasioExistente.Email = dto.Email!;
        if (!string.IsNullOrWhiteSpace(dto.Telefono)) gimnasioExistente.Telefono = dto.Telefono!;
        if (!string.IsNullOrWhiteSpace(dto.Cuil)) gimnasioExistente.Cuil = dto.Cuil!;  //VER SI NORMALIZAR O NO

            await _context.SaveChangesAsync();
        return gimnasioExistente;
    }

    public async Task<bool> EliminarGimnasio(long id)
    {
        var gimnasio = await _context.Gimnasios.FindAsync(id);
        if (gimnasio == null)
        {
            return false;
        }
        _context.Gimnasios.Remove(gimnasio);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Gimnasio?> ObtenerPorAdministradorIdAsync(long adminId)
    {
        return await _context.Gimnasios
            .Include(g => g.Administrador)
            .FirstOrDefaultAsync(g => g.AdministradorId == adminId);
    }

   
}
