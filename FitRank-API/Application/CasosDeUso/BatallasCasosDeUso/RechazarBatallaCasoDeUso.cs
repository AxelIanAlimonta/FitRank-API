using FitRank_API.Domain.Enums;
using FitRank_API.Infrastructure.Persistence;

namespace FitRank_API.Application.CasosDeUso.BatallasCasosDeUso
{
    public class RechazarBatallaCasoDeUso
    {
        private readonly FitRankDbContext _context;

        public RechazarBatallaCasoDeUso(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Ejecutar(int batallaId)
        {
            var batalla = await _context.Batallas.FindAsync(batallaId);
            if (batalla == null) return false;

            batalla.Estado = BatallaEstado.Rechazada;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
