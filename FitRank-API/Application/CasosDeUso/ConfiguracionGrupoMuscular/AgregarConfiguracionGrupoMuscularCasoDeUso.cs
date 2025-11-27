using AutoMapper;
using FitRank_API.Application.DTOs.ConfiguracionGrupoMuscularDTOs;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ConfiguracionGrupoMuscular
{
    public class AgregarConfiguracionGrupoMuscularCasoDeUso
    {
        private readonly IConfiguracionGrupoMuscularRepositorio _configuracionGrupoMuscularRepositorio;
        private readonly IMapper _mapper;

        public AgregarConfiguracionGrupoMuscularCasoDeUso(IConfiguracionGrupoMuscularRepositorio configuracionGrupoMuscularRepositorio, IMapper mapper)
        {
            _configuracionGrupoMuscularRepositorio = configuracionGrupoMuscularRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<ConfiguracionGrupoMuscularDTO> Ejecutar(AgregarConfiguracionGrupoMuscularDTO agregarConfiguracionGrupoMuscularDTO)
        {
            var configuracionGrupoMuscularEntidad = _mapper.Map<Domain.Entities.ConfiguracionGrupoMuscular>(agregarConfiguracionGrupoMuscularDTO);
            var configuracionGrupoMuscularCreado = await _configuracionGrupoMuscularRepositorio.AgregarAsync(configuracionGrupoMuscularEntidad);
            return _mapper.Map<ConfiguracionGrupoMuscularDTO>(configuracionGrupoMuscularCreado);
        }
    }
}
