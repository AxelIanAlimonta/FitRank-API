using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioDTOs;
using FitRank_API.Application.DTOs.EjercicioDTOs.ActualizarEjercicioDTO;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.EjercicioCasosDeUso;

public class ActualizarEjercicioCasoDeUso
{
    private readonly IEjercicioRepositorio _ejercicioRepositorio;
    private readonly IMapper _mapper;

    public ActualizarEjercicioCasoDeUso(IEjercicioRepositorio ejercicioRepositorio, IMapper mapper)
    {
        _ejercicioRepositorio = ejercicioRepositorio;
        _mapper = mapper;
    }

    public async Task<Ejercicio?> Ejecutar(ActualizarEjercicioDTO ejercicioDTO)
    {
        var ejercicioMapeado = _mapper.Map<Ejercicio>(ejercicioDTO);
        var resultado = await _ejercicioRepositorio.ActualizarEjercicioAsync(ejercicioMapeado);
        return resultado;
    }
}
