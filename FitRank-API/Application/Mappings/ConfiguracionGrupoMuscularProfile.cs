using AutoMapper;
using FitRank_API.Application.DTOs.ConfiguracionGrupoMuscularDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class ConfiguracionGrupoMuscularProfile : Profile
    {
           public ConfiguracionGrupoMuscularProfile()
        {
            CreateMap<ConfiguracionGrupoMuscular, ConfiguracionGrupoMuscularDTO>().ReverseMap();
            CreateMap<AgregarConfiguracionGrupoMuscularDTO, ConfiguracionGrupoMuscular>(); // No necesita reverse ya que agregar no posee ID
        }
    }
}
