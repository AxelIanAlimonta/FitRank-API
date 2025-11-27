using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.UseCases.Entrenamiento
{
    public class EliminarEntrenamientoCasoDeUso
    {
        private readonly IEntrenamientoRepositorio _repo;

        public EliminarEntrenamientoCasoDeUso(IEntrenamientoRepositorio repo)
        {
            _repo = repo;
        }

        public virtual async Task<bool> Ejecutar(long id)
        {
            return await _repo.EliminarAsync(id);
        }
    }
}
