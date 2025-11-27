using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.GrupoMuscularDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;

public class ActualizarGrupoMuscularCasoDeUso
{
    private readonly IGrupoMuscularRepositorio _grupoMuscularRepositorio;
    private readonly IMapper _mapper;

    public ActualizarGrupoMuscularCasoDeUso(IGrupoMuscularRepositorio grupoMuscularRepositorio, IMapper mapper)
    {
        _grupoMuscularRepositorio = grupoMuscularRepositorio;
        _mapper = mapper;
    }

    public virtual async Task<ObtenerGrupoMuscularDTO?> Ejecutar(ActualizarGrupoMuscularDTO grupoMuscularDTO)
    {
        var grupoMuscularEntidad = _mapper.Map<GrupoMuscular>(grupoMuscularDTO);
        var grupoMuscularActualizado = await _grupoMuscularRepositorio.ActualizarAsync(grupoMuscularEntidad);
        return grupoMuscularActualizado == null ? null : _mapper.Map<ObtenerGrupoMuscularDTO>(grupoMuscularActualizado);
    }


}
