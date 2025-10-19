using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.EjercicioAsignadoCasoDeUso;

public class ObtenerEjercicioAsignadoPorIdCasoDeUso
{
    private readonly IEjercicioAsignadoRepositorio _ejercicioAsignadoRepositorio;
    private readonly IMapper _mapper;
    public ObtenerEjercicioAsignadoPorIdCasoDeUso(IEjercicioAsignadoRepositorio ejercicioAsignadoRepositorio, IMapper mapper)
    {
        _ejercicioAsignadoRepositorio = ejercicioAsignadoRepositorio;
        _mapper = mapper;
    }

    public async Task<ObtenerEjercicioAsignadoDTO?> Ejecutar(long ejercicioAsignadoId)
    {
        var ejercicioAsignado = await _ejercicioAsignadoRepositorio.ObtenerPorIdAsync(ejercicioAsignadoId);
        if (ejercicioAsignado == null)
        {
            return null;
        }
        return _mapper.Map<ObtenerEjercicioAsignadoDTO>(ejercicioAsignado);
    }
}
