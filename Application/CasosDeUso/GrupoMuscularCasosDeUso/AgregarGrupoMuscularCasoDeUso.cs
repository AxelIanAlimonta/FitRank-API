using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.GrupoMuscularDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;

public class AgregarGrupoMuscularCasoDeUso
{
    private readonly IGrupoMuscularRepositorio _grupoMuscularRepositorio;
    private readonly IMapper _mapper;

    public AgregarGrupoMuscularCasoDeUso(IGrupoMuscularRepositorio grupoMuscularRepositorio, IMapper mapper)
    {
        _grupoMuscularRepositorio = grupoMuscularRepositorio;
        _mapper = mapper;
    }

    public virtual async Task<ObtenerGrupoMuscularDTO> Ejecutar(AgregarGrupoMuscularDTO agregarGrupoMuscularDTO)
    {
        var grupoMuscularEntidad = _mapper.Map<GrupoMuscular>(agregarGrupoMuscularDTO);
        var grupoMuscularCreado = await _grupoMuscularRepositorio.AgregarAsync(grupoMuscularEntidad);
        return _mapper.Map<ObtenerGrupoMuscularDTO>(grupoMuscularCreado);
    }


}
