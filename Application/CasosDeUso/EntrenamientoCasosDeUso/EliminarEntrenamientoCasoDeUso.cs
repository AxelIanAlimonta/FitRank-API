using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.UseCases.Entrenamiento
{
    public class EliminarEntrenamientoCasoDeUso
    {
        private readonly IEntrenamientoRepositorio _repo;

        public EliminarEntrenamientoCasoDeUso(IEntrenamientoRepositorio repo)
        {
            _repo = repo;
        }

        public async Task Ejecutar(long id)
        {
            await _repo.EliminarAsync(id);
        }
    }
}
