using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SocioCasoDeUso;

public class EliminarSocioCasoDeUso
{
    private readonly ISocioRepositorio _socioRepositorio;
    public EliminarSocioCasoDeUso(ISocioRepositorio socioRepositorio)
    {
        _socioRepositorio = socioRepositorio;
    }
    public async Task<bool> Ejecutar(long id)
    {
        return await _socioRepositorio.EliminarSocioAsync(id);
    }
}
