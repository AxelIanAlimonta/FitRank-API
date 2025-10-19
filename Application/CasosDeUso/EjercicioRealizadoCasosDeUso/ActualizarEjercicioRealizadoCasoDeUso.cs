using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioRealizadoDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.EjercicioRealizadoCasosDeUso;

public class ActualizarEjercicioRealizadoCasoDeUso
{
    private readonly IEjercicioRealizadoRepositorio _ejercicioRealizadoRepositorio;
    private readonly IMapper _mapper;

    public ActualizarEjercicioRealizadoCasoDeUso(IEjercicioRealizadoRepositorio ejercicioRealizadoRepositorio, IMapper mapper)
    {
        _ejercicioRealizadoRepositorio = ejercicioRealizadoRepositorio;
        _mapper = mapper;
    }

    public async Task<ObtenerEjercicioRealizadoDTO?> Ejecutar(ActualizarEjercicioRealizadoDTO ejercicioRealizadoActualizado)
    {
        var ejercicioRealizadoEntidad = _mapper.Map<EjercicioRealizado>(ejercicioRealizadoActualizado);
        var ejercicioRealizadoActualizadoEntidad = await _ejercicioRealizadoRepositorio.Actualizar(ejercicioRealizadoEntidad);
        if (ejercicioRealizadoActualizadoEntidad == null)
        {
            return null;
        }
        return _mapper.Map<ObtenerEjercicioRealizadoDTO>(ejercicioRealizadoActualizadoEntidad);
    }

}
