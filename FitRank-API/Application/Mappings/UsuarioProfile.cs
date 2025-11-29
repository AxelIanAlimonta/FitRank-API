using AutoMapper;

using FitRank_API.Application.DTOs.NotificacionDTOs;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Application.DTOs.UsuarioDTOs.ValidarAuth;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            CreateMap<Usuario, UsuarioAuthDTO>().ReverseMap();
            CreateMap<Usuario, RegisterDTO>().ReverseMap();
            CreateMap<Usuario, LoginDTO>().ReverseMap();
            CreateMap<Usuario, AuthResponseDTO>().ReverseMap();
            CreateMap<Usuario, EmailDTO>().ReverseMap();
            CreateMap<Usuario, EmailResponseDTO>().ReverseMap();
            CreateMap<Usuario, ValidarActivacionDTO>().ReverseMap();
            CreateMap<Usuario, ActivarResponseDTO>().ReverseMap();
            CreateMap<Usuario, ActivarCuentaDTO>().ReverseMap();

            CreateMap<Usuario, UsuarioNotificacionDTO>()
                .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.GetType().Name))
                .ForMember(dest => dest.FotoUrl, opt => opt.MapFrom(src => ""));
            
            CreateMap<Socio, UsuarioNotificacionDTO>()
                .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => "Socio"))
                .ForMember(dest => dest.FotoUrl, opt => opt.MapFrom(src => ""));
            
            CreateMap<Profesor, UsuarioNotificacionDTO>()
                .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => "Profesor"))
                .ForMember(dest => dest.FotoUrl, opt => opt.MapFrom(src => ""));
            
            CreateMap<Administrador, UsuarioNotificacionDTO>()
                .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => "Administrador"))
                .ForMember(dest => dest.FotoUrl, opt => opt.MapFrom(src => ""));
        }
    }
}
