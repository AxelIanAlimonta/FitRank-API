using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RutinaCasosDeUso
{
    public class MarcarDesmarcarRutinaFavoritaCasoDeUso
    {
        private readonly IRutinaRepositorio _repo;

        public MarcarDesmarcarRutinaFavoritaCasoDeUso(IRutinaRepositorio repo)
        {
            _repo = repo;
        }

        public virtual async Task<bool> Ejecutar(long rutinaId, bool favorita)
        {
            return await _repo.MarcarFavoritaAsync(rutinaId, favorita);
        }
    }
}
