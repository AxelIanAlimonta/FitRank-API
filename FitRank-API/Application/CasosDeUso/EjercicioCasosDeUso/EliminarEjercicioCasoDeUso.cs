using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.EjercicioCasosDeUso;

public class EliminarEjercicioCasoDeUso
{
    private readonly IEjercicioRepositorio _ejercicioRepositorio;
    public EliminarEjercicioCasoDeUso(IEjercicioRepositorio ejercicioRepositorio)
    {
        _ejercicioRepositorio = ejercicioRepositorio;
    }

    public async Task<bool> EjecutarAsync(long id)
    {
        return await _ejercicioRepositorio.EliminarEjercicioAsync(id);
    }

}
