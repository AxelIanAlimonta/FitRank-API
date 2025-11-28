using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.EjercicioAsignadoCasoDeUso;

public class EliminarEjercicioAsignadoCasoDeUso
{
    private readonly IEjercicioAsignadoRepositorio _ejercicioAsignadoRepositorio;
    public EliminarEjercicioAsignadoCasoDeUso(IEjercicioAsignadoRepositorio ejercicioAsignadoRepositorio)
    {
        _ejercicioAsignadoRepositorio = ejercicioAsignadoRepositorio;
    }
    public virtual async Task<bool> Ejecutar(long ejercicioAsignadoId)
    {
        return await _ejercicioAsignadoRepositorio.EliminarAsync(ejercicioAsignadoId);
    }
}
