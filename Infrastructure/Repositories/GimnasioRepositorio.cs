using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class GimnasioRepositorio : IGimnasioRepositorio
    {
        private readonly FitRankDbContext _context;
        public GimnasioRepositorio(FitRankDbContext context)
        {
            _context = context;
        }

        public Task<List<Logro>> ListarLogrosActivosAsync(int idGimnasio) => _context.GimnasioLogros.AsNoTracking()
                .Where(gl => gl.GimnasioId == idGimnasio && gl.Activo)
                .Select(gl => gl.Logro)
                .OrderBy(l => l.Nombre)
                .ToListAsync();

        public async Task SetEstadoLogro(int idGimnasio, int logroId, bool activo)
        {
            var gimnasioLogro = await _context.GimnasioLogros
                .FirstOrDefaultAsync(gl => gl.GimnasioId == idGimnasio && gl.LogroId == logroId);
            if (gimnasioLogro == null)
            {
                throw new KeyNotFoundException("El logro no está asociado al gimnasio.");
            }
            if(gimnasioLogro.Activo == activo)
            {
                return; // No hay cambio necesario
            }
            gimnasioLogro.Activo = activo;
            await _context.SaveChangesAsync();
        }

        public async Task OtorgarLogroAsync(int socioId, int logroId, int gimnasioId)
        {
            // Validaciones
            var socio = await _context.Socios.AsNoTracking()
                .Where(s => s.Id == socioId)
                .Select(s => new { s.Id, s.GimnasioId})
                .FirstOrDefaultAsync()
                ?? throw new InvalidOperationException("El socio no existe.");

            if (socio.GimnasioId != gimnasioId)
                throw new InvalidOperationException("El socio no pertenece al gimnasio.");

            var asignadaActiva = await _context.GimnasioLogros.AsNoTracking()
                .AnyAsync(gl => gl.GimnasioId == gimnasioId && gl.LogroId == logroId && gl.Activo);
            if (!asignadaActiva)
                throw new InvalidOperationException("El logro no está activo o no pertenece al gimnasio.");

            var logro = await _context.Logros.AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == logroId)
                ?? throw new InvalidOperationException("El logro no existe.");
            if (!logro.Activo)
                throw new InvalidOperationException("El logro está inactivo globalmente.");

            var yaOtorgado = await _context.SocioRealizaLogros.AsNoTracking()
                .AnyAsync(srl => srl.SocioId == socioId
                              && srl.LogroId == logroId
                              && srl.GimnasioId == gimnasioId);
            if (yaOtorgado)
                throw new InvalidOperationException("El logro ya ha sido otorgado al socio.");

            // Otorgarmiento de logro
            var otorgamiento = new SocioRealizaLogro
            {
                SocioId = socioId,
                LogroId = logroId,
                GimnasioId = gimnasioId,
                PuntosOtorgados = logro.PuntosOtorgados,
                FechaOtorgado = DateTime.UtcNow
            };

            _context.SocioRealizaLogros.Add(otorgamiento);
            await _context.SaveChangesAsync();
        }

    }
}
