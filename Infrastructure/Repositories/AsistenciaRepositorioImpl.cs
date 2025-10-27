using FitRank_API.Application.DTOs.Asistencia;
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
    

      public async Task<List<AsistenciaPorDiaDTO>> ObtenerConteoPorDiaAsync(int gimnasioId, DateTime? desde = null, DateTime? hasta = null)
        {
            var query = _context.Asistencias
                .Where(a => a.GimnasioId == gimnasioId);

            if (desde.HasValue)
                query = query.Where(a => a.Fecha >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(a => a.Fecha <= hasta.Value);

            var resultado = await query
                .GroupBy(a => a.Fecha.Date)
                .Select(g => new AsistenciaPorDiaDTO
                {
                    Fecha = g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Fecha)
                .ToListAsync();

            return resultado;
        }
    

    public async Task<List<AsistenciaDetalleUsuarioDTO>> ObtenerAsistenciasDetalladasPorUsuarioAsync(int usuarioId)
        {
            return await _context.Asistencias
                .Include(a => a.Gimnasio)
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.Fecha)
                .Select(a => new AsistenciaDetalleUsuarioDTO
                {
                    Fecha = a.Fecha,
                    HoraEntrada = a.HoraEntrada.TimeOfDay,
                    HoraSalida = a.HoraSalida.HasValue ? a.HoraSalida.Value.TimeOfDay : null,
                    Observaciones = a.Observaciones,
                    GimnasioNombre = a.Gimnasio.Nombre
                })
                .ToListAsync();
        }

    }
}
