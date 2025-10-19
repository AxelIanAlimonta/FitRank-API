using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.EjercicioRealizadoCasosDeUso;

public class EliminarEjercicioRealizadoCasoDeUso
{
    private readonly IEjercicioRealizadoRepositorio _ejercicioRealizadoRepositorio;
    public EliminarEjercicioRealizadoCasoDeUso(IEjercicioRealizadoRepositorio ejercicioRealizadoRepositorio)
    {
        _ejercicioRealizadoRepositorio = ejercicioRealizadoRepositorio;
    }
    public async Task<bool> Ejecutar(long id)
    {
        return await _ejercicioRealizadoRepositorio.EliminarAsync(id);
    }
}
