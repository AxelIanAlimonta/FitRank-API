using AutoMapper;

namespace FitRank_API.Application.Mappings;

public class GrupoMuscularProfile : Profile
{
    public GrupoMuscularProfile()
    {
        CreateMap<Domain.Entities.GrupoMuscular, DTOs.GrupoMuscularDTOs.GrupoMuscularDTO>().ReverseMap();
        CreateMap<Domain.Entities.GrupoMuscular, DTOs.GrupoMuscularDTOs.AgregarGrupoMuscularDTO>().ReverseMap();
    }

}
