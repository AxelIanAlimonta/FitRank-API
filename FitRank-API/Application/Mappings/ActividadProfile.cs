using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.ActividadDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.MappingProfiles
{
    public class ActividadProfile : Profile
    {
        public ActividadProfile()
        {
            CreateMap<AgregarActividadDTO, Actividad>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Entrenamiento, opt => opt.Ignore())
                .ForMember(dest => dest.EjercicioAsignado, opt => opt.Ignore())
                .ForMember(dest => dest.Serie, opt => opt.Ignore());

            CreateMap<ActualizarActividadDTO, Actividad>()
                .ForMember(dest => dest.Entrenamiento, opt => opt.Ignore())
                .ForMember(dest => dest.EjercicioAsignado, opt => opt.Ignore())
                .ForMember(dest => dest.Serie, opt => opt.Ignore());

            CreateMap<Actividad, ObtenerActividadDTO>();
        }
    }
}
