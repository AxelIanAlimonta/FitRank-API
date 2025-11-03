using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioDTOs;
using FitRank_API.Application.DTOs.EjercicioDTOs.AgregarEjercicioDTO;
using FitRank_API.Application.DTOs.EjercicioDTOs.ObtenerEjercicioDTO;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.EjercicioCasosDeUso;

public class AgregarEjercicioCasoDeUso
{
    private readonly IEjercicioRepositorio _ejercicioRepositorio;
    private readonly IMapper _mapper;

    public AgregarEjercicioCasoDeUso(IEjercicioRepositorio ejercicioRepositorio, IMapper mapper)
    {
        _ejercicioRepositorio = ejercicioRepositorio;
        _mapper = mapper;
    }

    public virtual async Task<ObtenerEjercicioDTO> Ejecutar(AgregarEjercicioDTO ejercicioDTO)
    {
        var ejercicio = _mapper.Map<Ejercicio>(ejercicioDTO);
        var ejercicioAgregado = await _ejercicioRepositorio.AgregarEjercicioAsync(ejercicio);
        return _mapper.Map<ObtenerEjercicioDTO>(ejercicioAgregado);
    }
}
