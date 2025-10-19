using AutoMapper;
using FitRank_API.Application.DTOs.RutinaEjercicioDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RutinaEjerciciosCasosDeUso;

public class AgregarRutinaEjercicioCasoDeUso
{
    private readonly IRutinaEjercicioRepositorio _rutinaEjercicioRepositorio;
    private readonly IMapper _mapper;

    public AgregarRutinaEjercicioCasoDeUso(IRutinaEjercicioRepositorio rutinaEjercicioRepositorio, IMapper mapper)
    {
        _rutinaEjercicioRepositorio = rutinaEjercicioRepositorio;
        _mapper = mapper;
    }

    public IRutinaEjercicioRepositorio RutinaEjercicioRepositorio => _rutinaEjercicioRepositorio;

    public async Task<ObtenerRutinaEjercicioDTO> Ejecutar(AgregarRutinaEjercicioDTO agregarRutinaEjercicioDTO)
    {
        var rutinaEjercicioEntidad = _mapper.Map<Domain.Entities.RutinaEjercicio>(agregarRutinaEjercicioDTO);
        var rutinaEjercicioCreado = await RutinaEjercicioRepositorio.Crear(rutinaEjercicioEntidad);
        return _mapper.Map<ObtenerRutinaEjercicioDTO>(rutinaEjercicioCreado);
    }
}
