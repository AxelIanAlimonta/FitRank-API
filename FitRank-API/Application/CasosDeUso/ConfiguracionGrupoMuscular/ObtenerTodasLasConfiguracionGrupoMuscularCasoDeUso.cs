using AutoMapper;
using FitRank_API.Application.DTOs.ConfiguracionGrupoMuscularDTOs;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ConfiguracionGrupoMuscular
{
    public class ObtenerTodasLasConfiguracionGrupoMuscularCasoDeUso
    {
        private readonly IConfiguracionGrupoMuscularRepositorio _configuracionGrupoMuscularRepositorio;
        private readonly IMapper _mapper;

        public ObtenerTodasLasConfiguracionGrupoMuscularCasoDeUso(IConfiguracionGrupoMuscularRepositorio configuracionGrupoMuscularRepositorio, IMapper mapper)
        {
            _configuracionGrupoMuscularRepositorio = configuracionGrupoMuscularRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<List<ConfiguracionGrupoMuscularDTO>> Ejecutar()
        {
            var configuracionesGrupoMuscular = await _configuracionGrupoMuscularRepositorio.ObtenerTodosAsync();
            return _mapper.Map<List<ConfiguracionGrupoMuscularDTO>>(configuracionesGrupoMuscular);
        }
    }
}
