using AutoMapper;
using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Application.DTOs.SocioDTOs;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings;

public class SocioProfile : Profile
{
    public SocioProfile()
    {
        CreateMap<Socio, SocioDTO>().ReverseMap();
        CreateMap<AgregarSocioDTO, Socio>()
                  .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) 
                  .ForMember(dest => dest.Id, opt => opt.Ignore())
                  .ForMember(dest => dest.Puntaje, opt => opt.Ignore())
                  .ForMember(dest => dest.FechaRegistro, opt => opt.MapFrom(_ => DateTime.UtcNow));
        CreateMap<Socio, UsuarioAuthDTO>().ReverseMap();

        CreateMap<Socio, UsuarioAuthDTO>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
              .ForMember(dest => dest.Apellidos, opt => opt.MapFrom(src => src.Apellido))
              .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
              .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => src.NombreUsuario))
              .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.Rol))
              .ForMember(dest => dest.CuotaPagadaHasta, opt => opt.MapFrom(src => src.CuotaPagadaHasta))
              .ForMember(dest => dest.TieneCuotaPagada, opt => opt.MapFrom(src =>
                  src.CuotaPagadaHasta.HasValue && src.CuotaPagadaHasta > DateTime.Now))
              .ReverseMap();

    
        CreateMap<RegisterInvitacionDTO, Socio>()
            .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.NombreUsuario))
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => "Activo"))
            .ForMember(dest => dest.FechaRegistro, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => "User"))
            .ForMember(dest => dest.GimnasioId, opt => opt.Ignore()) 
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
    

    // 🔹 Mapeo de Socio → SocioDTO
    CreateMap<Socio, SocioDTO>()
                // Mapea propiedades del padre (Usuario)
                .ForMember(dest => dest.FotoUrl, opt => opt.MapFrom(src => src.FotoDePerfil))
                // Mapea propiedades del hijo (Socio)
                .ForMember(dest => dest.GimnasioId, opt => opt.MapFrom(src => src.GimnasioId ?? 0))
                .ForMember(dest => dest.GimnasioNombre, opt => opt.MapFrom(src => src.Gimnasio.Nombre));
        }
}
