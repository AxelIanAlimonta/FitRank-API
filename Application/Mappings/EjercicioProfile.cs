using AutoMapper;
using FitRank_API.Domain.Entities;
using FitRank_API.Application.DTOs.Ejercicionamespace;

namespace FitRank_API.Application.Mappings;
public class EjercicioProfile:Profile
{
    public EjercicioProfile()
    {
        CreateMap<Ejercicio, EjercicioDTO>().ReverseMap();
        CreateMap<Ejercicio, CrearEjercicioDTO>().ReverseMap();
    }
}
