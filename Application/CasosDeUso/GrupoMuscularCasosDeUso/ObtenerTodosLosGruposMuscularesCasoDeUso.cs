using AutoMapper;
using FitRank_API.Infrastructure.Interfaces;

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

    public async Task<List<DTOs.GrupoMuscularDTOs.GrupoMuscularDTO>> Ejecutar()
    {
        var gruposMusculares = await _grupoMuscularRepositorio.ObtenerTodosAsync();
        return _mapper.Map<List<DTOs.GrupoMuscularDTOs.GrupoMuscularDTO>>(gruposMusculares);
    }


}
