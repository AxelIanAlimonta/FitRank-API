using AutoMapper;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;

public class OtenerRutinaPorIdCasoDeUso
{
    private readonly IRutinaRepositorio _rutinaRepositorio;
    private readonly IMapper _mapper;

    public OtenerRutinaPorIdCasoDeUso(IRutinaRepositorio rutinaRepositorio, IMapper mapper)
    {
        _rutinaRepositorio = rutinaRepositorio;
        _mapper = mapper;
    }

    public async Task<ObtenerRutinaDTO?> Ejecutar(long id)
    {
        var rutina = await _rutinaRepositorio.ObtenerPorId(id);
        if (rutina == null)
        {
            return null;
        }
        return _mapper.Map<ObtenerRutinaDTO>(rutina);

    }
}
