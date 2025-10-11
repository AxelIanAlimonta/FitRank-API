using AutoMapper;
using FitRank_API.Application.DTOs.Usuario;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile() {

            CreateMap<Domain.Entities.Usuario, DTOs.Usuario.CrearUsuarioDTO>().ReverseMap();
            CreateMap<Domain.Entities.Usuario, DTOs.Usuario.UsuarioDTO>().ReverseMap();
            CreateMap<Domain.Entities.Usuario, DTOs.Usuario.UpDateUsuarioDTO>().ReverseMap();
            CreateMap<Domain.Entities.Usuario, DTOs.Usuario.UsuariorRespuestaDto>().ReverseMap();



        }
    }
}
