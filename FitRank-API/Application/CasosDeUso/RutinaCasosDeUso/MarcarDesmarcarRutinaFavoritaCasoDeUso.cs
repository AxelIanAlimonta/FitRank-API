using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RutinaCasosDeUso
{
    public class MarcarDesmarcarRutinaFavoritaCasoDeUso
    {
        private readonly IRutinaRepositorio _repo;

        public MarcarDesmarcarRutinaFavoritaCasoDeUso(IRutinaRepositorio repo)
        {
            _repo = repo;
        }

        public async Task<bool> Ejecutar(long rutinaId, bool favorita)
        {
            return await _repo.MarcarFavoritaAsync(rutinaId, favorita);
        }
    }
}
