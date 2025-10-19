using AutoMapper;
using FitRank_API.Application.DTOs.SesionRealizadaDeEjercicios;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings;

public class SesionRealizadaDeEjerciciosProfile: Profile
{
    public SesionRealizadaDeEjerciciosProfile()
    {
        CreateMap<SesionRealizadaDeEjercicios, SesionRealizadaDeEjerciciosDTO>().ReverseMap();
        CreateMap<SesionRealizadaDeEjercicios, AgregarSesionRealizadaDeEjerciciosDTO>().ReverseMap();
    }
}
