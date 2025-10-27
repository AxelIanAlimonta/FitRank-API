using AutoMapper;
using FitRank_API.Application.DTOs.DiaDeLaSemanaDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class DiaDeLaSemanaProfile : Profile
    {
        public DiaDeLaSemanaProfile()
        {
            CreateMap<DiaDeLaSemana, DiaDeLaSemanaDTO>().ReverseMap();
            CreateMap<DiaDeLaSemana, AgregarDiaDeLaSemanaDTO>().ReverseMap();
            CreateMap<DiaDeLaSemana, ActualizarDiaDeLaSemanaDTO>().ReverseMap();
        }
    }
}
