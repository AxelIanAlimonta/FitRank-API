using AutoMapper;
using FitRank_API.Application.DTOs.RutinaEjercicioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RutinaEjerciciosCasosDeUso;

public class ActualizarRutinaEjercicioCasoDeUso
{
    private readonly IRutinaEjercicioRepositorio _rutinaEjercicioRepositorio;
    private readonly IMapper _mapper;

    public ActualizarRutinaEjercicioCasoDeUso(IRutinaEjercicioRepositorio rutinaEjercicioRepositorio, IMapper mapper)
    {
        _rutinaEjercicioRepositorio = rutinaEjercicioRepositorio;
        _mapper = mapper;
    }

    public async Task<ObtenerRutinaEjercicioDTO?> Ejecutar(ActualizarRutinaEjercicioDTO actualizarRutinaEjercicioDTO)
    {
        var rutinaEjercicioEntidad = _mapper.Map<RutinaEjercicio>(actualizarRutinaEjercicioDTO);
        var rutinaEjercicioActualizado = await _rutinaEjercicioRepositorio.Actualizar(rutinaEjercicioEntidad);
        if (rutinaEjercicioActualizado == null) return null;
        return _mapper.Map<ObtenerRutinaEjercicioDTO>(rutinaEjercicioActualizado);

    }

}
