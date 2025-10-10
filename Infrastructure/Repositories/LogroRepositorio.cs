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

        public async Task<int> CrearLogroAsync(Logro entity, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(entity.Nombre))
                throw new ArgumentException("El nombre del logro no puede estar vacío.");
            if (string.IsNullOrWhiteSpace(entity.Descripcion)) 
                throw new ArgumentException("La descripción del logro no puede estar vacía.");
            if (entity.PuntosOtorgados <= 0) 
                throw new ArgumentException("Los puntos otorgados deben ser mayores a cero.");

            await _context.Logros.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
            return entity.Id;
        }

        public Task<List<Logro>> ListarActivosAsync(CancellationToken ct = default)
            => _context.Logros.AsNoTracking()
                .Where(l => l.Activo)
                .OrderBy(l => l.Nombre)
                .ToListAsync(ct);

        public Task<List<SocioRealizaLogro>> MisLogrosAsync(int socioId, CancellationToken ct = default)
            => _context.SocioRealizaLogros.AsNoTracking()
                .Where(srl => srl.SocioId == socioId)
                .Include(srl => srl.Logro)
                .OrderByDescending(srl => srl.FechaOtorgado)
                .ToListAsync(ct);

        public async Task<SocioRealizaLogro?> OtorgarSiNoExisteAsync(int socioId, int logroId, CancellationToken ct = default)
        {
            var logro = await _context.Logros.FirstOrDefaultAsync(l => l.Id == logroId, ct);
            if (logro is null) throw new InvalidOperationException($"Logro {logroId} inexistente.");
            if (!logro.Activo) throw new InvalidOperationException($"Logro '{logro.Nombre}' inactivo globalmente.");

            var tiene = await _context.SocioRealizaLogros
                .AsNoTracking()
                .AnyAsync(x => x.SocioId == socioId && x.LogroId == logroId, ct);

            if (tiene)
                return null;

            var otorgado = SocioRealizaLogro.Crear(socioId, logro);
            _context.SocioRealizaLogros.Add(otorgado);

            try
            {
                await _context.SaveChangesAsync(ct);
                return otorgado;
            }
            catch (DbUpdateException ex) when (ex.Message.Contains("unique", StringComparison.OrdinalIgnoreCase))
            {
                // otro proceso lo otorgó al mismo tiempo
                return null;
            }
        }


        public async Task SetActivoAsync(int logroId, bool activo, CancellationToken ct = default)
        {
            var l = await _context.Logros.FirstOrDefaultAsync(x => x.Id == logroId, ct)
                    ?? throw new KeyNotFoundException($"Logro {logroId} no encontrado.");

            if (l.Activo != activo)
            {
                l.Activo = activo;
                await _context.SaveChangesAsync(ct);
            }
        }
    }
}
