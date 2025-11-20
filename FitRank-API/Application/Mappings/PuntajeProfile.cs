using AutoMapper;
using FitRank_API.Application.DTOs.PuntajeDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class PuntajeProfile : Profile
    {
        public PuntajeProfile() 
        {
            CreateMap<Puntaje, ObtenerPuntajeDTO>().ReverseMap();
            CreateMap<Puntaje, AgregarPuntajeDTO>().ReverseMap();
            CreateMap<Puntaje, ActualizarPuntajeDTO>().ReverseMap();

        }
    }
}
