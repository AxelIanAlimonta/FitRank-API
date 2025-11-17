using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.EjercicioAsignadoCasoDeUso;

public class AgregarEjercicioAsignadoCasoDeUso
{
    private readonly IEjercicioAsignadoRepositorio _ejercicioAsignadoRepositorio;
    private readonly IMapper _mapper;

    public AgregarEjercicioAsignadoCasoDeUso(IEjercicioAsignadoRepositorio ejercicioAsignadoRepositorio, IMapper mapper)
    {
        _ejercicioAsignadoRepositorio = ejercicioAsignadoRepositorio;
        _mapper = mapper;
    }

    public virtual async Task<ObtenerEjercicioAsignadoDTO> Ejecutar(AgregarEjercicioAsignadoDTO nuevoEjercicioAsignado)
    {
        var ejercicioAsignadoEntidad = _mapper.Map<EjercicioAsignado>(nuevoEjercicioAsignado);
        var ejercicioAsignadoAgregado = await _ejercicioAsignadoRepositorio.AgregarAsync(ejercicioAsignadoEntidad);
        return _mapper.Map<ObtenerEjercicioAsignadoDTO>(ejercicioAsignadoAgregado);
    }


}
