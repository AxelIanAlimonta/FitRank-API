using AutoMapper;
using FitRank_API.Application.DTOs.NotificacionDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class NotificacionProfile : Profile
    {
        public NotificacionProfile()
        {
            CreateMap<AgregarNotificacionDTO, Notificacion>()
                .ForMember(dest => dest.UsuarioReceptorId, opt => opt.MapFrom(src => src.UsuarioReceptorId))
                .ForMember(dest => dest.UsuarioEmisorId, opt => opt.Ignore());
            CreateMap<Notificacion, ObtenerNotificacionDTO>();

            CreateMap<Usuario, UsuarioNotificacionDTO>()
    .ForMember(dest => dest.NombreCompleto,
               opt => opt.MapFrom(src => $"{src.Nombre} {src.Apellido}"))
    .ForMember(dest => dest.Rol,
               opt => opt.MapFrom(src => src.Rol.ToString()));

        }
    }
}
