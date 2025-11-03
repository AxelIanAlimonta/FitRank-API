using AutoMapper;
using FitRank_API.Application.DTOs.DificultadDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.DificultadCasosDeUso;

public class ActualizarDificultadCasoDeUso
{
    private readonly IDificultadRepositorio _dificultadRepositorio;
    private readonly IMapper _mapper;

    public ActualizarDificultadCasoDeUso(IDificultadRepositorio dificultadRepositorio, IMapper mapper)
    {
        _dificultadRepositorio = dificultadRepositorio;
        _mapper = mapper;
    }

    public async Task<DificultadDTO?> Ejecutar(DificultadDTO dificultadDTO)
    {
        var dificultadEntidad = _mapper.Map<Dificultad>(dificultadDTO);
        var dificultadActualizada = await _dificultadRepositorio.ActualizarAsync(dificultadEntidad);
        return dificultadActualizada == null ? null : _mapper.Map<DificultadDTO>(dificultadActualizada);
    }

}
