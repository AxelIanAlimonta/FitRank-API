using AutoMapper;
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

    public async Task<DTOs.GrupoMuscularDTOs.GrupoMuscularDTO?> Ejecutar(long id)
    {
        var grupoMuscular = await _grupoMuscularRepositorio.ObtenerPorIdAsync(id);
        return grupoMuscular == null ? null : _mapper.Map<DTOs.GrupoMuscularDTOs.GrupoMuscularDTO>(grupoMuscular);
    }
}
