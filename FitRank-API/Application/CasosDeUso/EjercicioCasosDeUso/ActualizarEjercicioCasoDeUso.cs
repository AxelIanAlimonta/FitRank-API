using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioDTOs;
using FitRank_API.Application.DTOs.EjercicioDTOs.ActualizarEjercicioDTO;
using FitRank_API.Application.DTOs.EjercicioDTOs.ObtenerEjercicioDTO;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

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

    public virtual async Task<ObtenerEjercicioDTO?> Ejecutar(ActualizarEjercicioDTO ejercicioDTO)
    {
        var ejercicioExistente = await _ejercicioRepositorio.ObtenerEjercicioPorIdAsync(ejercicioDTO.Id);
        if (ejercicioExistente == null)
        {
            return null; 
        }

        _mapper.Map(ejercicioDTO, ejercicioExistente);
        await _ejercicioRepositorio.ActualizarEjercicioAsync(ejercicioExistente);

        return _mapper.Map<ObtenerEjercicioDTO>(ejercicioExistente);

    }
}
