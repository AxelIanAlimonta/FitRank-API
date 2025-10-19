using AutoMapper;
using FitRank_API.Application.DTOs.SerieRealizadaDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class SerieRealizadaProfile : Profile
    {
        public SerieRealizadaProfile() 
        { 
            CreateMap<SerieRealizada, ObtenerSerieRealizadaDTO>().ReverseMap();
            CreateMap<SerieRealizada, AgregarSerieRealizadaDTO>().ReverseMap();
            CreateMap<SerieRealizada, ActualizarSerieRealizadaDTO>().ReverseMap();

        }

    }
}
