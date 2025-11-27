using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class PuntajeRepositorioImpl : IPuntajeRepositorio
    {
        private readonly FitRankDbContext _context;
        public PuntajeRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<List<Puntaje>> ObtenerTodasAsync()
        {
            return await _context.Puntajes
                .ToListAsync();
        }
        public async Task<Puntaje?> ObtenerPorIdAsync(long id)
        {
            return await _context.Puntajes
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Puntaje> AgregarAsync(Puntaje puntaje)
        {
            _context.Puntajes.Add(puntaje);
            await _context.SaveChangesAsync();
            return puntaje;
        }
        public async Task<Puntaje?> ActualizarAsync(Puntaje puntaje)
        {
            var existingPuntaje = await _context.Puntajes.FindAsync(puntaje.Id);
            if (existingPuntaje == null)
            {
                return null;
            }
            existingPuntaje.SocioId = puntaje.SocioId;
            existingPuntaje.Motivo = puntaje.Motivo;
            existingPuntaje.Fecha = puntaje.Fecha;
            existingPuntaje.Valor = puntaje.Valor;


            await _context.SaveChangesAsync();
            return existingPuntaje;
        }
        public async Task<bool> EliminarAsync(long id)
        {
            var puntaje = await _context.Puntajes.FindAsync(id);
            if (puntaje == null)
            {
                return false;
            }
            _context.Puntajes.Remove(puntaje);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
