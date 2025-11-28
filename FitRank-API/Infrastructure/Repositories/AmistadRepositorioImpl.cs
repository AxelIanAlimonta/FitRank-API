using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class AmistadRepositorioImpl : IAmistadRepositorio
    {
        private readonly FitRankDbContext _context;

        public AmistadRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }
        
        public async Task<Amistad> CrearAsync(Amistad amistad)
        {
            _context.Amistades.Add(amistad);
            await _context.SaveChangesAsync();
            return amistad;
        }

        public async Task<bool> EliminarAsync(Amistad amistad)
        {
            _context.Amistades.Remove(amistad);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Amistad>> ObtenerPorSocioIdAsync(long socioId, EstadoAmistad estado)
        {
            return await _context.Amistades
            .Include(a => a.Socio1)
            .Include(a => a.Socio2)
            .Where(a =>
                a.Estado == estado &&
                (a.SocioId1 == socioId || a.SocioId2 == socioId))
            .ToListAsync();
        }

        public async Task<Amistad?> ObtenerPorIdAsync(long id)
        {
            return await _context.Amistades.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Amistad?> ObtenerPorIdDeSociosAsync(long socioId1, long socioId2)
        {
            return await _context.Amistades.FirstOrDefaultAsync(a =>
                (a.SocioId1 == socioId1 && a.SocioId2 == socioId2) ||
                (a.SocioId1 == socioId2 && a.SocioId2 == socioId1));
        }

        public async Task<Amistad> ActualizarAsync(Amistad amistad)
        {
            _context.Amistades.Update(amistad);
            await _context.SaveChangesAsync();
            return amistad;
        }


        public async Task<List<Amistad>> ObtenerSolicitudesPendientesAsync(long socioId)
        {
            return await _context.Amistades
            .Include(a => a.Solicitante)
            .Where(a =>
                a.Estado == EstadoAmistad.Pendiente &&
                (a.SocioId1 == socioId || a.SocioId2 == socioId) &&
                a.SolicitanteId != socioId)
            .ToListAsync();
        }
    }
}