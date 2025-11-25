using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class SerieProfile : Profile
    {
        public SerieProfile()
        {

            CreateMap<AgregarSerieDTO, Serie>().ReverseMap();
            CreateMap<ActualizarSerieDTO, Serie>().ReverseMap();
            CreateMap<ObtenerSerieDTO, Serie>().ReverseMap();
        }
    }
}
