using AutoMapper;
using FitRank_API.Application.DTOs.GimnasioDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings;

public class GimnasioProfile : Profile
{
    public GimnasioProfile()
    {
        CreateMap<Gimnasio, ObtenerGimnasioDTO>().ReverseMap();
        CreateMap<Gimnasio, AgregarGimnasioDTO>().ReverseMap();
        CreateMap<Gimnasio, ActualizarGimnasioDTO>().ReverseMap();
    }
}
