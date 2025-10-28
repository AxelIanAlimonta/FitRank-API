using AutoMapper;
using FitRank_API.Application.DTOs.GrupoMuscularDTOs;
using FitRank_API.Application.DTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings;

public class GrupoMuscularProfile : Profile
{
    public GrupoMuscularProfile()
    {
        CreateMap<GrupoMuscular, ObtenerGrupoMuscularDTO>().ReverseMap();
        CreateMap<AgregarGrupoMuscularDTO, GrupoMuscular>();
        CreateMap<ActualizarGrupoMuscularDTO, GrupoMuscular>().ReverseMap();
    }

}
