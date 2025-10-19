using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioRealizadoDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.EjercicioRealizadoCasosDeUso;

public class AgregarEjercicioRealizadoCasoDeUso
{

    private readonly IEjercicioRealizadoRepositorio _ejercicioRealizadoRepositorio;
    private readonly IMapper _mapper;

    public AgregarEjercicioRealizadoCasoDeUso(IEjercicioRealizadoRepositorio ejercicioRealizadoRepositorio, IMapper mapper)
    {
        _ejercicioRealizadoRepositorio = ejercicioRealizadoRepositorio;
        _mapper = mapper;
    }

    public async Task<ObtenerEjercicioRealizadoDTO> Ejecutar(AgregarEjercicioRealizadoDTO nuevoEjercicioRealizado)
    {
        var ejercicioRealizadoEntidad = _mapper.Map<Domain.Entities.EjercicioRealizado>(nuevoEjercicioRealizado);
        var ejercicioRealizadoAgregado = await _ejercicioRealizadoRepositorio.Agregar(ejercicioRealizadoEntidad);
        return _mapper.Map<ObtenerEjercicioRealizadoDTO>(ejercicioRealizadoAgregado);
    }
}
