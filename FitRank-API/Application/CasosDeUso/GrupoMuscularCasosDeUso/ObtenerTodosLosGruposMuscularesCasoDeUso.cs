using AutoMapper;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;

public class ObtenerTodosLosGruposMuscularesCasoDeUso
{
    private readonly IGrupoMuscularRepositorio _grupoMuscularRepositorio;
    private readonly IMapper _mapper;

    public ObtenerTodosLosGruposMuscularesCasoDeUso(IGrupoMuscularRepositorio grupoMuscularRepositorio, IMapper mapper)
    {
        _grupoMuscularRepositorio = grupoMuscularRepositorio;
        _mapper = mapper;
    }

    public virtual async Task<List<DTOs.GrupoMuscularDTOs.ObtenerGrupoMuscularDTO>> Ejecutar()
    {
        var gruposMusculares = await _grupoMuscularRepositorio.ObtenerTodosAsync();
        return _mapper.Map<List<DTOs.GrupoMuscularDTOs.ObtenerGrupoMuscularDTO>>(gruposMusculares);
    }


}
