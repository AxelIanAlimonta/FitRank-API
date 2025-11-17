using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SocioCasoDeUso
{
    public class CambiarParticipacionRankingCasoDeUso
    {
        private readonly ISocioRepositorio _socioRepositorio;

        public CambiarParticipacionRankingCasoDeUso(ISocioRepositorio socioRepositorio)
        {
            _socioRepositorio = socioRepositorio;
        }

        public async Task<bool> Ejecutar(long socioId, bool participa)
        {
            return await _socioRepositorio.CambiarParticipacionRankingAsync(socioId, participa);
        }
    }
}
