using AutoMapper;
using FitRank_API.Application.DTOs.AdministradorDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
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
                .ForMember(dest => dest.Gimnasio, opt => opt.MapFrom(src => src.GimnasioId))

             
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Rol, opt => opt.Ignore())
                .ForMember(dest => dest.EsActivado, opt => opt.Ignore())
                .ForMember(dest => dest.Gimnasio, opt => opt.Ignore());
        }
    }
}
