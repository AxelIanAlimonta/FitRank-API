using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.EntrenamientoDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.MappingProfiles
{
    public class EntrenamientoProfile : Profile
    {
        public EntrenamientoProfile()
        {
            CreateMap<AgregarEntrenamientoDTO, Entrenamiento>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Actividades, opt => opt.Ignore())
                .ForMember(dest => dest.Socio, opt => opt.Ignore());

            CreateMap<ActualizarEntrenamientoDTO, Entrenamiento>()
                .ForMember(dest => dest.Actividades, opt => opt.Ignore())
                .ForMember(dest => dest.Socio, opt => opt.Ignore());

            CreateMap<Entrenamiento, ObtenerEntrenamientoDTO>();
        }
    }
}
