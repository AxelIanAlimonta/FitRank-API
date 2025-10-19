using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioDTOs;
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

    public async Task<List<EjercicioDTO>> EjecutarAsync()
    {
        var ejercicios = await _ejercicioRepositorio.ObtenerEjerciciosAsync();
        return _mapper.Map<List<EjercicioDTO>>(ejercicios);
    }
}
