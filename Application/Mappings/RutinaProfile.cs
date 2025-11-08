using AutoMapper;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class RutinaProfile : Profile
    {
        public RutinaProfile()
        {
            CreateMap<Rutina, ObtenerRutinaDTO>().ReverseMap();
            CreateMap<AgregarRutinaDTO, Rutina>()
                .ForMember(dest => dest.SocioId, opt => opt.MapFrom(src => src.SocioId))
                .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId));

            CreateMap<Rutina, AgregarRutinaDTO>()
                .ForMember(dest => dest.SocioId, opt => opt.MapFrom(src => src.SocioId))
                .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.UsuarioId));
            CreateMap<Rutina, ActualizarRutinaDTO>().ReverseMap();

            CreateMap<Rutina, RutinaProfesorDTO>()
          .ForMember(dest => dest.SocioNombre, opt => opt.MapFrom(src =>
              src.Socio != null ? src.Socio.Nombre + " " + src.Socio.Apellido : null))
          .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.TipoCreacion));


        }
    }

}
