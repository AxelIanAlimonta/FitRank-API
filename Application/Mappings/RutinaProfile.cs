using AutoMapper;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings;

public class RutinaProfile : Profile
{
    public RutinaProfile()
    {
        CreateMap<Rutina, ObtenerRutinaDTO>().ReverseMap();
        CreateMap<Rutina, AgregarRutinaDTO>().ReverseMap();
        CreateMap<Rutina, ActualizarRutinaDTO>().ReverseMap();
    }
}
