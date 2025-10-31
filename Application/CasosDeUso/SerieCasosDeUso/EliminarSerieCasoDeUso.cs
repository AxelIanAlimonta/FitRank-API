using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SerieCasosDeUso
{
    public class EliminarSerieCasoDeUso
    {
        private readonly ISerieRepositorio _repo;
        public EliminarSerieCasoDeUso(ISerieRepositorio repo)
        {
            _repo = repo;
        }

        public async Task<bool> Ejecutar(long id)
        {
            return await _repo.EliminarAsync(id);
        }
    }
}
