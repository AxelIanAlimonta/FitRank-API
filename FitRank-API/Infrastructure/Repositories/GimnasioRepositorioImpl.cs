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

    public async Task<Gimnasio?> ActualizarGimnasio(Gimnasio gimnasio)
    {
        var existingGimnasio = await _context.Gimnasios.FindAsync(gimnasio.Id);
        if (existingGimnasio == null)
        {
            return null;
        }

        existingGimnasio.Nombre = gimnasio.Nombre;
        existingGimnasio.Direccion = gimnasio.Direccion;
        existingGimnasio.RazonSocial = gimnasio.RazonSocial;
        existingGimnasio.LogoUrl = gimnasio.LogoUrl;
        existingGimnasio.ColorPrincipal = gimnasio.ColorPrincipal;
        existingGimnasio.ColorSecundario = gimnasio.ColorSecundario;
        existingGimnasio.Email = gimnasio.Email;
        existingGimnasio.Telefono = gimnasio.Telefono;
        existingGimnasio.Cuil = gimnasio.Cuil;
        existingGimnasio.AdministradorId = gimnasio.AdministradorId;
        

        await _context.SaveChangesAsync();
        return existingGimnasio;

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

    public long? ObtenerGimnasioIdPorUsuario(long userId)
    {
      
        var admin = _context.Administradores.FirstOrDefault(a => a.Id == userId);
        if (admin != null)
        {
            var gimnasio = _context.Gimnasios.FirstOrDefault(g => g.AdministradorId == admin.Id);
            if (gimnasio != null)
                return gimnasio.Id;

        }

       
        var profesor = _context.Profesores.FirstOrDefault(p => p.Id == userId);
        if (profesor != null)
            return profesor.GimnasioId;

      
        var socio = _context.Socios.FirstOrDefault(s => s.Id == userId);
        if (socio != null)
            return socio.GimnasioId;

       
        return null;
    }
    public async Task<Gimnasio?> ActualizarPersonalizacion(long id, string colorPrincipal, string colorSecundario, string? logoUrl)
    {
        var gym = await _context.Gimnasios.FindAsync(id);
        if (gym == null)
            return null;

       
        gym.ColorPrincipal = colorPrincipal;
        gym.ColorSecundario = colorSecundario;

        if (!string.IsNullOrWhiteSpace(logoUrl))
            gym.LogoUrl = logoUrl;

        await _context.SaveChangesAsync();
        return gym;
    }

}
