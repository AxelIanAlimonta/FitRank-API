using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.EjercicioCasosDeUso;

public class ObtenerEjercicioPorIdCasoDeUso
{
    private readonly IEjercicioRepositorio _ejercicioRepositorio;
    private readonly IMapper _mapper;

    public ObtenerEjercicioPorIdCasoDeUso(IEjercicioRepositorio ejercicioRepositorio, IMapper mapper)
    {
        _ejercicioRepositorio = ejercicioRepositorio;
        _mapper = mapper;
    }

    public async Task<EjercicioDTO?> EjecutarAsync(long id)
    {
        var ejercicio = await _ejercicioRepositorio.ObtenerEjercicioPorIdAsync(id);
        return ejercicio == null ? null : _mapper.Map<EjercicioDTO>(ejercicio);
    }
}
