using AutoMapper;
using FitRank_API.Application.DTOs.Logro;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class LogroProfile : Profile
    {
        public LogroProfile()
        {
            // Entidad → DTO
            CreateMap<Logro, LogroDto>()
                .ForMember(dest => dest.Puntos, opt => opt.MapFrom(src => src.PuntosOtorgados));

            // Entidad SocioRealizaLogro → DTO de logros del socio
            CreateMap<SocioRealizaLogro, LogroUsuarioDto>()
                .ForMember(dest => dest.LogroId, opt => opt.MapFrom(src => src.LogroId))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Logro!.Nombre))
                .ForMember(dest => dest.PuntosOtorgados, opt => opt.MapFrom(src => src.PuntosOtorgados))
                .ForMember(dest => dest.FechaOtorgado, opt => opt.MapFrom(src => src.FechaOtorgado));
        }
    }
}
