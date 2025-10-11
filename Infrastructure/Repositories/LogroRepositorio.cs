using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FitRank_API.Infrastructure.Repositories
{
    public class LogroRepositorio : ILogroRepositorio
    {
        private readonly FitRankDbContext _context;
        public LogroRepositorio(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<int> CrearLogroAsync(Logro entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Nombre))
                throw new ArgumentException("El nombre del logro no puede estar vacío.");
            if (string.IsNullOrWhiteSpace(entity.Descripcion)) 
                throw new ArgumentException("La descripción del logro no puede estar vacía.");
            if (entity.PuntosOtorgados <= 0) 
                throw new ArgumentException("Los puntos otorgados deben ser mayores a cero.");

            await _context.Logros.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public Task<List<Logro>> ListarAsync()
            => _context.Logros.AsNoTracking()
                .OrderBy(l => l.Nombre)
                .ToListAsync();

        public async Task SetActivoAsync(int logroId, bool activo)
        {
            var l = await _context.Logros.FirstOrDefaultAsync(x => x.Id == logroId)
                    ?? throw new KeyNotFoundException($"Logro {logroId} no encontrado.");

            if (l.Activo != activo)
            {
                l.Activo = activo;
                await _context.SaveChangesAsync();
            }
        }

        public Task <Logro?> ObtenerPorIdAsync(int logroId)
            => _context.Logros.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == logroId);
    }
}
