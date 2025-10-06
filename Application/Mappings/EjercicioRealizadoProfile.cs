using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioRealizado;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class EjercicioRealizadoProfile : Profile
    {
        public EjercicioRealizadoProfile()
        {


            CreateMap<EjercicioRealizado, EjercicioRealizadoDTOSalida>()
        .ForMember(dest => dest.NombreEjercicio, opt => opt.MapFrom(src => src.Ejercicio.Nombre))
        .ForMember(dest => dest.GrupoMuscular, opt => opt.MapFrom(src => src.Ejercicio.GrupoMuscular))
        .ForMember(dest => dest.Series, opt => opt.MapFrom(src => src.Series))
        .ForMember(dest => dest.Repeticiones, opt => opt.MapFrom(src => src.Repeticiones))
        .ForMember(dest => dest.Peso, opt => opt.MapFrom(src => src.Peso));
        }


    }
}
