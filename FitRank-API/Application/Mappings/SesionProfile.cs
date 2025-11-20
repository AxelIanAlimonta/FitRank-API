using AutoMapper;
using FitRank_API.Domain.Entities;

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
