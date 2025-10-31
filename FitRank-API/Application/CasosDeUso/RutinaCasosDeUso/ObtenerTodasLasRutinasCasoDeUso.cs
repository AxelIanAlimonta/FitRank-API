using AutoMapper;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;

public class ObtenerTodasLasRutinasCasoDeUso
{
    private readonly IRutinaRepositorio _rutinaRepositorio;
    private readonly IMapper _mapper;

    public ObtenerTodasLasRutinasCasoDeUso(IRutinaRepositorio rutinaRepositorio, IMapper mapper)
    {
        _rutinaRepositorio = rutinaRepositorio;
        _mapper = mapper;
    }
    public async Task<List<ObtenerRutinaDTO>> Ejecutar()
    {
        var rutinas = await _rutinaRepositorio.ObtenerTodasAsync();
        return _mapper.Map<List<ObtenerRutinaDTO>>(rutinas);
    }

}
