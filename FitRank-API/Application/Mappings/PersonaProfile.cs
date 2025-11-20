using AutoMapper;
using FitRank_API.Domain.Entities;
using FitRank_API.Application.DTOs.Persona;

namespace FitRank_API.Application.Mappings;

public class PersonaProfile : Profile
{
    public PersonaProfile()
    {
        CreateMap<Persona, PersonaDTO>().ReverseMap();
        CreateMap<Persona, UpdatePersonaDTO>().ReverseMap();
        CreateMap<Persona, CreatePersonaDTO>().ReverseMap();
    }

}
