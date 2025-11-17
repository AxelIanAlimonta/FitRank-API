using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioDTOs.ActualizarEjercicioDTO;
using FitRank_API.Application.DTOs.EjercicioDTOs.AgregarEjercicioDTO;
using FitRank_API.Application.DTOs.EjercicioDTOs.ObtenerEjercicioDTO;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings;

public class EjercicioProfile : Profile
{
    public EjercicioProfile()
    {
        CreateMap<Ejercicio, ObtenerEjercicioDTO>()
            .ForMember(dest => dest.NombreGrupoMuscular, opt => opt.MapFrom(src => src.GrupoMuscular.Nombre))
            .ForMember(dest => dest.NombreMaquina, opt => opt.MapFrom(src => src.Maquina != null ? src.Maquina.Nombre : null));

        CreateMap<Ejercicio, AgregarEjercicioDTO>().ReverseMap();
        CreateMap<Ejercicio, ActualizarEjercicioDTO>().ReverseMap();
    }
}
