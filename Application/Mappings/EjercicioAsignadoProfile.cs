using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings;

public class EjercicioAsignadoProfile : Profile
{
    public EjercicioAsignadoProfile()
    {
        CreateMap<EjercicioAsignado, ObtenerEjercicioAsignadoDTO>().ReverseMap();
        CreateMap<EjercicioAsignado, AgregarEjercicioAsignadoDTO>().ReverseMap();
        CreateMap<EjercicioAsignado, ActualizarEjercicioAsignadoDTO>().ReverseMap();
    }
}
