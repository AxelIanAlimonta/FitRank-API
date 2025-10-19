using AutoMapper;
using FitRank_API.Application.DTOs.RutinaEjercicioDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RutinaEjerciciosCasosDeUso;

public class ObtenerRutinaEjercicioPorIdCasoDeUso
{
    private readonly IRutinaEjercicioRepositorio _rutinaEjercicioRepositorio;
    private readonly IMapper _mapper;

    public ObtenerRutinaEjercicioPorIdCasoDeUso(IRutinaEjercicioRepositorio rutinaEjercicioRepositorio, IMapper mapper)
    {
        _rutinaEjercicioRepositorio = rutinaEjercicioRepositorio;
        _mapper = mapper;
    }

    public async Task<ObtenerRutinaEjercicioDTO?> Ejecutar(long id)
    {
        var rutinaEjercicio = await _rutinaEjercicioRepositorio.ObtenerPorId(id);
        if (rutinaEjercicio == null) return null;
        return _mapper.Map<ObtenerRutinaEjercicioDTO>(rutinaEjercicio);
    }
}
