using AutoMapper;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SesionCasosDeUso;

public class EliminarSesionCasoDeUso
{
    private readonly ISesionRepositorio _sesionRepositorio;
    private readonly IMapper _mapper;
    public EliminarSesionCasoDeUso(ISesionRepositorio sesionRepositorio)
    {
        _sesionRepositorio = sesionRepositorio;
    }
    public virtual async Task<bool> Ejecutar(long id)
    {
        return await _sesionRepositorio.EliminarAsync(id);
    }
}
