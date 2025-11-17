using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;

public class EliminarRutinaCasoDeUso
{
    private readonly IRutinaRepositorio _rutinaRepositorio;
    public EliminarRutinaCasoDeUso(IRutinaRepositorio rutinaRepositorio)
    {
        _rutinaRepositorio = rutinaRepositorio;
    }
    public virtual async Task<bool> Ejecutar(long id)
    {
        return await _rutinaRepositorio.EliminarAsync(id);
    }
}
