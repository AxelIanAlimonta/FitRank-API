using FitRank_API.Domain.Enums;
using FitRank_API.Infrastructure.Persistence;

namespace FitRank_API.Application.CasosDeUso.BatallasCasosDeUso
{
    public class AceptarBatallaCasoDeUso
    {
        private readonly FitRankDbContext _context;

        public AceptarBatallaCasoDeUso(FitRankDbContext context)
        {
            _context = context;
        }

        public virtual async Task<bool> Ejecutar(int id)
        {
            var batalla = await _context.Batallas.FindAsync(id);
            if (batalla == null) return false;

            batalla.Estado = BatallaEstado.Activa;
            batalla.FechaInicio = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
