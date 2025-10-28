using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
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

        public async Task ActualizarAsync(Asistencia asistencia)
        {
            _context.Asistencias.Update(asistencia);
            await _context.SaveChangesAsync();
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



    }
}
