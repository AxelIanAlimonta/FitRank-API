using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioRealizadoDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.EjercicioRealizadoCasosDeUso;

public class ObtenerTodosLosEjercicioRealizadoCasoDeUso
{
    private readonly IEjercicioRealizadoRepositorio _ejercicioRealizadoRepositorio;
    private readonly IMapper _mapper;

    public ObtenerTodosLosEjercicioRealizadoCasoDeUso(IEjercicioRealizadoRepositorio ejercicioRealizadoRepositorio, IMapper mapper)
    {
        _ejercicioRealizadoRepositorio = ejercicioRealizadoRepositorio;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ObtenerEjercicioRealizadoDTO>> Ejecutar()
    {
        var ejerciciosRealizadosEntidad = await _ejercicioRealizadoRepositorio.ObtenerTodos();
        return _mapper.Map<IEnumerable<ObtenerEjercicioRealizadoDTO>>(ejerciciosRealizadosEntidad);
    }
}
