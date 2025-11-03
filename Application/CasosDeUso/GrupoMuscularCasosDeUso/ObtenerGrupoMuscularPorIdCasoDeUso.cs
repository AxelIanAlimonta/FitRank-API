using AutoMapper;
using FitRank_API.Application.DTOs.GrupoMuscularDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;

public class ObtenerGrupoMuscularPorIdCasoDeUso
{
    private readonly IGrupoMuscularRepositorio _grupoMuscularRepositorio;
    private readonly IMapper _mapper;

    public ObtenerGrupoMuscularPorIdCasoDeUso(IGrupoMuscularRepositorio grupoMuscularRepositorio, IMapper mapper)
    {
        _grupoMuscularRepositorio = grupoMuscularRepositorio;
        _mapper = mapper;
    }


    public virtual async Task<ObtenerGrupoMuscularDTO?> Ejecutar(long id)
    {
        var grupoMuscular = await _grupoMuscularRepositorio.ObtenerPorIdAsync(id);
        return grupoMuscular == null ? null : _mapper.Map<ObtenerGrupoMuscularDTO>(grupoMuscular);
    }
}
