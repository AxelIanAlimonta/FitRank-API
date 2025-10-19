using AutoMapper;
using FitRank_API.Application.DTOs.RutinaEjercicioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RutinaEjerciciosCasosDeUso;

public class ObtenerTodasRutinasEjerciciosCasoDeUso
{
    private readonly IRutinaEjercicioRepositorio _rutinaEjercicioRepositorio;
    private IMapper _mapper;
    public ObtenerTodasRutinasEjerciciosCasoDeUso(IRutinaEjercicioRepositorio rutinaEjercicioRepositorio, IMapper mapper)
    {
        _rutinaEjercicioRepositorio = rutinaEjercicioRepositorio;
        _mapper = mapper;
    }

    public async Task<List<ObtenerRutinaEjercicioDTO>> Ejecutar()
    {
        var rutinasEjercicios = await _rutinaEjercicioRepositorio.ObtenerTodos();
        return _mapper.Map<List<ObtenerRutinaEjercicioDTO>>(rutinasEjercicios);
    }

}
