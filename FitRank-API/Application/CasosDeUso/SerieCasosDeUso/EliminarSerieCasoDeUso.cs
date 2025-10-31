using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.UseCases.Serie
{
    public class EliminarSerieCasoDeUso
    {
        private readonly ISerieRepositorio _repo;
        public EliminarSerieCasoDeUso(ISerieRepositorio repo)
        {
            _repo = repo;
        }

        public async Task Ejecutar(long id)
        {
            await _repo.EliminarAsync(id);
        }
    }
}
