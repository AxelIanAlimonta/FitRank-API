using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioRealizadoDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class EjercicioRealizadoProfile : Profile
    {
        public EjercicioRealizadoProfile()
        {
            CreateMap<EjercicioRealizado, ObtenerEjercicioRealizadoDTO>().ReverseMap();
            CreateMap<EjercicioRealizado, AgregarEjercicioRealizadoDTO>().ReverseMap();
            CreateMap<EjercicioRealizado, ActualizarEjercicioRealizadoDTO>().ReverseMap();
        }
    }
}
