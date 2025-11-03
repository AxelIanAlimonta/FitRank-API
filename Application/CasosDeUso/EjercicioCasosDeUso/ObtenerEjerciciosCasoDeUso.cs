using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioDTOs.ObtenerEjercicioDTO;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.EjercicioCasosDeUso;

public class ObtenerEjerciciosCasoDeUso
{
    private readonly IEjercicioRepositorio _ejercicioRepositorio;
    private readonly IMapper _mapper;
    public ObtenerEjerciciosCasoDeUso(IEjercicioRepositorio ejercicioRepositorio, IMapper mapper)
    {
        _ejercicioRepositorio = ejercicioRepositorio;
        _mapper = mapper;
    }

    public virtual async Task<List<ObtenerEjercicioDTO>> EjecutarAsync()
    {
        var ejercicios = await _ejercicioRepositorio.ObtenerEjerciciosAsync();
        return _mapper.Map<List<ObtenerEjercicioDTO>>(ejercicios);
    }
}
