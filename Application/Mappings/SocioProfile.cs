using AutoMapper;
using FitRank_API.Application.DTOs.SocioDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings;

public class SocioProfile : Profile
{
    public SocioProfile()
    {
        CreateMap<Socio, SocioDTO>().ReverseMap();
        CreateMap<Socio, AgregarSocioDTO>().ReverseMap();
    }
}
