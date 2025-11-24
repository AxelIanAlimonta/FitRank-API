using AutoMapper;
using FitRank_API.Application.DTOs.ConfiguracionGrupoMuscularDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ConfiguracionGrupoMuscular
{
    public class ActualizarConfiguracionGrupoMuscularCasoDeUso
    {
        private readonly IConfiguracionGrupoMuscularRepositorio _configuracionGrupoMuscularRepositorio;
        private readonly IMapper _mapper;

        public ActualizarConfiguracionGrupoMuscularCasoDeUso(IConfiguracionGrupoMuscularRepositorio configuracionGrupoMuscularRepositorio, IMapper mapper)
        {
            _configuracionGrupoMuscularRepositorio = configuracionGrupoMuscularRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<ConfiguracionGrupoMuscularDTO?> Ejecutar(ConfiguracionGrupoMuscularDTO configuracionGrupoMuscularDTO)
        {
            var configuracionGrupoMuscularEntidad = _mapper.Map<Domain.Entities.ConfiguracionGrupoMuscular>(configuracionGrupoMuscularDTO);
            var configuracionGrupoMuscularActualizado = await _configuracionGrupoMuscularRepositorio.ActualizarAsync(configuracionGrupoMuscularEntidad);
            return configuracionGrupoMuscularActualizado == null ? null : _mapper.Map<ConfiguracionGrupoMuscularDTO>(configuracionGrupoMuscularActualizado);
        }
    }
}
