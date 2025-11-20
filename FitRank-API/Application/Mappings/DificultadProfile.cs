using AutoMapper;
using FitRank_API.Application.DTOs.DificultadDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class DificultadProfile : Profile
    {
        public DificultadProfile()
        {
            CreateMap<Dificultad, DificultadDTO>().ReverseMap();
            CreateMap<Dificultad, AgregarDificultadDTO>().ReverseMap();
        }
    }
}
