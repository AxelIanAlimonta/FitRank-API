using AutoMapper;
using FitRank_API.Application.DTOs.MedidaCorporalDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class MedidaCorporalProfile : Profile
    {
        public MedidaCorporalProfile()
        {
            CreateMap<AgregarMedidaCorporalDTO, MedidaCorporal>();
            CreateMap<ActualizarMedidaCorporalDTO, MedidaCorporal>();
            CreateMap<MedidaCorporal, ObtenerMedidaCorporalDTO>()
                .ForMember(dest => dest.NombreSocio, opt => opt.MapFrom(src => src.Socio.Nombre));
                
        }
    }
}
