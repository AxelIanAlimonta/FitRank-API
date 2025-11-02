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

        public async Task Ejecutar(long id)
        {
            await _repo.EliminarAsync(id);
        }
    }
}
