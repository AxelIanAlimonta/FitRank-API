using AutoMapper;
using FitRank_API.Domain.Entities;

using FitRank_API.Application.DTOs.IngresoDTOs;

namespace FitRank_API.Application.Mapping
{
    public class IngresoMappingProfile : Profile
    {
        public IngresoMappingProfile()
        {
            CreateMap<Ingreso, ObtenerIngresoDTO>()
                .ForMember(dest => dest.MetodoPago, opt => opt.MapFrom(src => src.MetodoPago.ToString()));

            CreateMap<AgregarIngresoDTO, Ingreso>()
                .ForMember(dest => dest.Fecha, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Confirmado, opt => opt.MapFrom(src => true));
        }
    }
}
