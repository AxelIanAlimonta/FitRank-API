using AutoMapper;
using FitRank_API.Application.DTOs.JornadaDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class JornadaProfile : Profile
    {
        public JornadaProfile()
        {
            CreateMap<Jornada, JornadaDTO>().ReverseMap();
            CreateMap<Jornada, AgregarJornadaDTO>().ReverseMap();
            CreateMap<Jornada, ActualizarJornadaDTO>().ReverseMap();
        }
    }
}
