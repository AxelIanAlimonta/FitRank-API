using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.EntrenamientoDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.MappingProfiles;

public class EntrenamientoProfile : Profile
{
    public EntrenamientoProfile()
    {
        CreateMap<AgregarEntrenamientoDTO, Entrenamiento>().ReverseMap();
        CreateMap<ActualizarEntrenamientoDTO, Entrenamiento>().ReverseMap();
        CreateMap<Entrenamiento, ObtenerEntrenamientoDTO>().ReverseMap();
    }
}
