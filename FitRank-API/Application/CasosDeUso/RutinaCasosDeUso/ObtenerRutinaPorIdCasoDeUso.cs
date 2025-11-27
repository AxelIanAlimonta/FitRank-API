using AutoMapper;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;

public class ObtenerRutinaPorIdCasoDeUso
{
    private readonly IRutinaRepositorio _rutinaRepositorio;
    private readonly IMapper _mapper;

    public ObtenerRutinaPorIdCasoDeUso(IRutinaRepositorio rutinaRepositorio, IMapper mapper)
    {
        _rutinaRepositorio = rutinaRepositorio;
        _mapper = mapper;
    }

    public virtual async Task<ObtenerRutinaDTO?> Ejecutar(long id)
    {
        var rutina = await _rutinaRepositorio.ObtenerPorIdAsync(id);
        if (rutina == null)
        {
            return null;
        }
        return _mapper.Map<ObtenerRutinaDTO>(rutina);

    }
}
