using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RutinaEjerciciosCasosDeUso;

public class EliminarRutinaEjercicioCasoDeUso
{
    private readonly IRutinaEjercicioRepositorio _rutinaEjercicioRepositorio;
    public EliminarRutinaEjercicioCasoDeUso(IRutinaEjercicioRepositorio rutinaEjercicioRepositorio)
    {
        _rutinaEjercicioRepositorio = rutinaEjercicioRepositorio;
    }

    public Task<bool> Ejecutar(long id)
    {
        return _rutinaEjercicioRepositorio.Eliminar(id);
    }

}
