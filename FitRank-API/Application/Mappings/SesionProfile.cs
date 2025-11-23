using AutoMapper;
using FitRank_API.Application.DTOs.SesionDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings;

public class SesionProfile : Profile
{
    public SesionProfile()
    {
        CreateMap<Sesion, ObtenerSesionDTO>()
            .ForMember(dest => dest.RutinaNombre, opt => opt.MapFrom(src => src.Rutina.Nombre)).ReverseMap();

        CreateMap<AgregarSesionDTO, Sesion>().ReverseMap();
        CreateMap<ActualizarSesionDTO, Sesion>().ReverseMap();
    }
}
