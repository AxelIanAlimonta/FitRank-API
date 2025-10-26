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
        public async Task<IEnumerable<Asistencia>> ObtenerPorUsuarioAsync(long usuarioId)
        {
            return await _context.Asistencias
                .Where(a => a.UsuarioId == usuarioId)
                .ToListAsync();
        }
        public async Task<Asistencia?> ObtenerPorIdAsync(long id)
        {
            return await _context.Asistencias.FindAsync(id);
        }
        public async Task ActualizarAsync(Asistencia asistencia)
        {
            _context.Asistencias.Update(asistencia);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Asistencia>> ObtenerTodasAsync()
        {
            return await _context.Asistencias.ToListAsync();
        }
    }


}
