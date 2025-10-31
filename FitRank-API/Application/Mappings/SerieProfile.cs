using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.MappingProfiles
{
    public class SerieProfile : Profile
    {
        public SerieProfile()
        {
           
            CreateMap<AgregarSerieDTO, Serie>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Actividades, opt => opt.Ignore())
                .ForMember(dest => dest.EjercicioAsignado, opt => opt.Ignore());

            
            CreateMap<Serie, ObtenerSerieDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.NumeroDeSerie, opt => opt.MapFrom(src => src.NumeroDeSerie))
                .ForMember(dest => dest.Duracion, opt => opt.MapFrom(src => src.Duracion))
                .ForMember(dest => dest.Repeticiones, opt => opt.MapFrom(src => src.Repeticiones))
                .ForMember(dest => dest.Peso, opt => opt.MapFrom(src => src.Peso))
                .ForMember(dest => dest.EjercicioAsignadoId, opt => opt.MapFrom(src => src.EjercicioAsignadoId));

         
            CreateMap<ActualizarSerieDTO, Serie>()
                .ForMember(dest => dest.EjercicioAsignado, opt => opt.Ignore())
                .ForMember(dest => dest.Actividades, opt => opt.Ignore());
        }
    }
}
