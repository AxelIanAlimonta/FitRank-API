using FitRank_API.Infrastructure.Persistence;

namespace FitRank_API.Application.CasosDeUso.BatallasCasosDeUso
{
    public class ObtenerBatallaPorIdCasoDeUso
    {
        private readonly FitRankDbContext _context;
        public ObtenerBatallaPorIdCasoDeUso(FitRankDbContext context)
        {
            _context = context;
        }
        public async Task<Domain.Entities.BatallaPunto?> Ejecutar(int batallaId)
        {
            var batalla = await _context.Batallas.FindAsync(batallaId);
            return batalla;
        }
    }
}
