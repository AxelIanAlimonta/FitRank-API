using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioDTOs.ObtenerEjercicioDTO;
using FitRank_API.Domain.Interfaces;

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

    public virtual async Task<ObtenerEjercicioDTO?> Ejecutar(long id)
    {
        var ejercicio = await _ejercicioRepositorio.ObtenerEjercicioPorIdAsync(id);
        return _mapper.Map<ObtenerEjercicioDTO>(ejercicio);
    }
}
