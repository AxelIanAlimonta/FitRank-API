using AutoMapper;
using FitRank_API.Application.DTOs.AdministradorDTOs;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class AdminProfile : Profile
    {
        public AdminProfile()


        {
            CreateMap<Administrador, ObtenerAdministradorDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => src.Telefono))
                .ForMember(dest => dest.Cuil, opt => opt.MapFrom(src => src.Cuil))
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.Direccion))
                .ForMember(dest => dest.Localidad, opt => opt.MapFrom(src => src.Localidad))
                .ReverseMap();
            CreateMap<AgregarAdministradorDTO, Administrador>()
            
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Apellido, opt => opt.MapFrom(src => src.Apellido))
                .ForMember(dest => dest.Dni, opt => opt.MapFrom(src => src.Dni))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => src.NombreUsuario))
                .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => src.Telefono))

              
                .ForMember(dest => dest.Cuil, opt => opt.MapFrom(src => src.Cuil))
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.Direccion))
                .ForMember(dest => dest.Localidad, opt => opt.MapFrom(src => src.Localidad))
                

             
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Rol, opt => opt.Ignore())
                .ForMember(dest => dest.EsActivado, opt => opt.Ignore())
                .ForMember(dest => dest.Gimnasio, opt => opt.Ignore());


            CreateMap<Administrador, UsuarioAuthDTO>()
    .IncludeBase<Usuario, UsuarioAuthDTO>() // hereda mapeo base
    .ForMember(dest => dest.GimnasioId, opt => opt.MapFrom(src => src.GimnasioId));
        }

    }
}
