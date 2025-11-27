using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SocioCasoDeUso;

public class EliminarSocioCasoDeUso
{
    private readonly ISocioRepositorio _socioRepositorio;
    public EliminarSocioCasoDeUso(ISocioRepositorio socioRepositorio)
    {
        _socioRepositorio = socioRepositorio;
    }

    public virtual async Task<bool> Ejecutar(long id)
    {
        return await _socioRepositorio.EliminarAsync(id);
    }

}
