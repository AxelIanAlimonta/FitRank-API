using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.UseCases.Actividad;

public class EliminarActividadCasoDeUso
{
    private readonly IActividadRepositorio _repo;

    public EliminarActividadCasoDeUso(IActividadRepositorio repo)
    {
        _repo = repo;
    }

    public virtual async Task<bool> Ejecutar(long id)
    {
        return await _repo.EliminarAsync(id);
    }
}
