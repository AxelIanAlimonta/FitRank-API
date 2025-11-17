using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.LogroCasosDeUso;

public class EliminarLogroCasoDeUso
{
    private readonly ILogroRepositorio _logroRepositorio;
    public EliminarLogroCasoDeUso(ILogroRepositorio logroRepositorio)
    {
        _logroRepositorio = logroRepositorio;
    }
    public virtual async Task<bool> Ejecutar(long id)
    {
        return await _logroRepositorio.EliminarLogro(id);
    }
}
