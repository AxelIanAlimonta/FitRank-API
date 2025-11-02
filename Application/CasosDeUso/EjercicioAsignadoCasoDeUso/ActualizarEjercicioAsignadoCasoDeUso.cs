using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.EjercicioAsignadoCasoDeUso;

public class ActualizarEjercicioAsignadoCasoDeUso
{
    private readonly IEjercicioAsignadoRepositorio _ejercicioAsignadoRepositorio;
    private readonly IMapper _mapper;
    public ActualizarEjercicioAsignadoCasoDeUso(IEjercicioAsignadoRepositorio ejercicioAsignadoRepositorio, IMapper mapper)
    {
        _ejercicioAsignadoRepositorio = ejercicioAsignadoRepositorio;
        _mapper = mapper;
    }

    public virtual async Task<ObtenerEjercicioAsignadoDTO?> Ejecutar(ActualizarEjercicioAsignadoDTO ejercicioAsignadoActualizado)
    {
        var ejercicioAsignadoEntidad = _mapper.Map<Domain.Entities.EjercicioAsignado>(ejercicioAsignadoActualizado);
        var ejercicioAsignado = await _ejercicioAsignadoRepositorio.ActualizarAsync(ejercicioAsignadoEntidad);
        if (ejercicioAsignado == null)
        {
            return null;
        }
        return _mapper.Map<ObtenerEjercicioAsignadoDTO>(ejercicioAsignado);

    }

}
