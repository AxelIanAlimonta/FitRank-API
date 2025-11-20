using AutoMapper;

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

        }
    }
}
