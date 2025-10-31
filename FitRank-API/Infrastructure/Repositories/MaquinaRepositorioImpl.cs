using FitRank_API.Application.DTOs.MaquinaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class MaquinaRepositorioImpl : IMaquinaRepositorio
    {
        private readonly FitRankDbContext _context;
        public MaquinaRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<Maquina?> ActualizarMaquina(Maquina maquina)
        {
            var maquinaExistente = await _context.Maquinas.FindAsync(maquina.Id);
            if (maquinaExistente == null)
            {
                return null;
            }

            maquinaExistente.Nombre = maquina.Nombre;
            maquinaExistente.GimnasioId = maquina.GimnasioId;
            maquinaExistente.UrlImagen = maquina.UrlImagen;
            maquinaExistente.Qr = maquina.Qr;

            await _context.SaveChangesAsync();
            return maquinaExistente;
        }

        public async Task<Maquina> AgregarMaquina(Maquina maquina)
        {
            _context.Maquinas.Add(maquina);
            await _context.SaveChangesAsync();
            return maquina;
        }

        public async Task<bool> EliminarMaquina(long id)
        {
            var maquina = await _context.Maquinas.FindAsync(id);
            if (maquina == null)
            {
                return false;
            }
            _context.Maquinas.Remove(maquina);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Maquina?> ObtenerMaquinaPorId(long id)
        {
            return await _context.Maquinas.FindAsync(id);
        }

        public async Task<List<Maquina>> ObtenerTodasLasMaquinas()
        {
            return await _context.Maquinas.ToListAsync();
        }
    }
}
