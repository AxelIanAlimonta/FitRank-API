using AutoMapper;
using FitRank_API.Application.DTOs.RankingDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class RankingProfile : Profile
    {
        public RankingProfile()
        {
            CreateMap<Socio, RankingDTO>()
                .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.Nombre + " " + src.Apellido))
                .ForMember(dest => dest.PuntajeTotal, opt => opt.Ignore()); // lo calculás en LINQ o servicio

            CreateMap<Socio, PosicionDTO>()
                .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.Nombre + " " + src.Apellido))
                .ForMember(dest => dest.PuntajeTotal, opt => opt.Ignore())
                .ForMember(dest => dest.Posicion, opt => opt.Ignore());

        }
    }
}
