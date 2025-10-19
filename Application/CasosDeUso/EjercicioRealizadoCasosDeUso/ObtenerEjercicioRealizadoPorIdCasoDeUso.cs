using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioRealizadoDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.EjercicioRealizadoCasosDeUso;

public class ObtenerEjercicioRealizadoPorIdCasoDeUso
{
    private readonly IEjercicioRealizadoRepositorio _ejercicioRealizadoRepositorio;
    private readonly IMapper _mapper;
    public ObtenerEjercicioRealizadoPorIdCasoDeUso(IEjercicioRealizadoRepositorio ejercicioRealizadoRepositorio, IMapper mapper)
    {
        _ejercicioRealizadoRepositorio = ejercicioRealizadoRepositorio;
        _mapper = mapper;
    }
    public async Task<ObtenerEjercicioRealizadoDTO?> Ejecutar(long id)
    {
        var ejercicioRealizadoEntidad = await _ejercicioRealizadoRepositorio.ObtenerPorId(id);
        if (ejercicioRealizadoEntidad == null)
        {
            return null;
        }
        return _mapper.Map<ObtenerEjercicioRealizadoDTO>(ejercicioRealizadoEntidad);
    }
}
