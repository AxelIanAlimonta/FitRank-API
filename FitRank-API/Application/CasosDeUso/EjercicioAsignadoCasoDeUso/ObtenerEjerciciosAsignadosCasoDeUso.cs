using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.EjercicioAsignadoCasoDeUso;

public class ObtenerEjerciciosAsignadosCasoDeUso
{
    private readonly IEjercicioAsignadoRepositorio _ejercicioAsignadoRepositorio;
    private readonly IMapper _mapper;
    public ObtenerEjerciciosAsignadosCasoDeUso(IEjercicioAsignadoRepositorio ejercicioAsignadoRepositorio, IMapper mapper)
    {
        _ejercicioAsignadoRepositorio = ejercicioAsignadoRepositorio;
        _mapper = mapper;
    }

    public async Task<List<ObtenerEjercicioAsignadoDTO>> Ejecutar()
    {
        var ejerciciosAsignados = await _ejercicioAsignadoRepositorio.ObtenerTodosAsync();
        return _mapper.Map<List<ObtenerEjercicioAsignadoDTO>>(ejerciciosAsignados);
    }
}
