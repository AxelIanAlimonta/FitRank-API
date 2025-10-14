using AutoMapper;
using FitRank_API.Application.DTOs.Ejercicionamespace;
using FitRank_API.Application.DTOs.RutinaNamespace;
using FitRank_API.Application.DTOs.RutinaNameSpace;
using FitRank_API.Domain.Entities;
using NUnit.Framework.Internal;

namespace FitRank_API.Application.Mappings
{
    public class RutinaProfile: Profile
    {
        public RutinaProfile()
        {

            // Mapeos Rutina
            CreateMap<Rutina, RutinaDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.UsuarioId))
                .ForMember(dest => dest.Ejercicios, opt => opt.MapFrom(src => src.Ejercicios))
                .ReverseMap();
            CreateMap<Rutina, EditarRutinaDTO>().ReverseMap();
            CreateMap<Rutina, CrearRutinaDTO>().ReverseMap();
            // Mapeos ejercicio

            CreateMap<Ejercicio, EjercicioDTO>().ReverseMap();
        }
    }
}
