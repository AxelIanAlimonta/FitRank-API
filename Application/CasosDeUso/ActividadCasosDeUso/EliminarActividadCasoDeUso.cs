using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.UseCases.Actividad
{
    public class EliminarActividadCasoDeUso
    {
        private readonly IActividadRepositorio _repo;

        public EliminarActividadCasoDeUso(IActividadRepositorio repo)
        {
            _repo = repo;
        }

        public async Task<bool> Ejecutar(long id)
        {
            return await _repo.EliminarAsync(id);
        }
    }
}
