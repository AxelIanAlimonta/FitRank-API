using AutoMapper;
using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class InvitacionProfile : Profile
    {
        public InvitacionProfile()
        {
 
            CreateMap<GenerarInvitacionDTO, Invitacion>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.MetodoPago, opt => opt.MapFrom(src => src.MetodoPago ?? "Efectivo"))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => "Pendiente"))
                .ForMember(dest => dest.DatosPrellenados, opt => opt.Ignore()) // se genera aparte como JSON
                .ForMember(dest => dest.CreadaEn, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.ExpiraEn, opt => opt.MapFrom(_ => DateTime.Now.AddHours(24)));



            CreateMap<FallbackEfectivoDTO, Invitacion>()
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(_ => "FallbackEfectivo"))
                .ForMember(dest => dest.MetodoPago, opt => opt.MapFrom(_ => "Efectivo"))
                .ForMember(dest => dest.CuotaPagadaHasta, opt => opt.MapFrom(_ => DateTime.Now.AddDays(30)));

            CreateMap<GenerarInvitacionDTO, Usuario>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellidos))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => src.Telefono))
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(_ => "User"))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(_ => "Activo"))
                .ForMember(dest => dest.EsActivado, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.TokenRecuperacion, opt => opt.Ignore())
                .ForMember(dest => dest.TokenExpira, opt => opt.Ignore());
        }
    }
}
