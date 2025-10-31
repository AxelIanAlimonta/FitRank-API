using AutoMapper;
using FitRank_API.Application.DTOs.ConfiguracionGrupoMuscularDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ConfiguracionGrupoMuscular
{
    public class ObtenerConfiguracionGrupoMuscularPorIdCasoDeUso
    {
        private readonly IConfiguracionGrupoMuscularRepositorio _configuracionGrupoMuscularRepositorio;
        private readonly IMapper _mapper;

        public ObtenerConfiguracionGrupoMuscularPorIdCasoDeUso(IConfiguracionGrupoMuscularRepositorio configuracionGrupoMuscularRepositorio, IMapper mapper)
        {
            _configuracionGrupoMuscularRepositorio = configuracionGrupoMuscularRepositorio;
            _mapper = mapper;
        }

        public async Task<ConfiguracionGrupoMuscularDTO?> Ejecutar(long id)
        {
            var configuracionGrupoMuscular = await _configuracionGrupoMuscularRepositorio.ObtenerPorIdAsync(id);
            return configuracionGrupoMuscular == null ? null : _mapper.Map<ConfiguracionGrupoMuscularDTO>(configuracionGrupoMuscular);
        }
    }
}
