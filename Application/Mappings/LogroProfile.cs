using AutoMapper;
using FitRank_API.Application.DTOs.Logro;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class LogroProfile : Profile
    {
        public LogroProfile()
        {
            // Lectura: Entidad -> LogroDto (PuntosOtorgados -> Puntos)
            CreateMap<Logro, LogroDto>()
                .ForMember(d => d.Puntos, o => o.MapFrom(s => s.PuntosOtorgados));

            // Escritura: LogroCreateDto -> Entidad (Puntos -> PuntosOtorgados)
            CreateMap<LogroCreateDto, Logro>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.PuntosOtorgados, o => o.MapFrom(s => s.Puntos));

            // Update: LogroUpdateDto -> Entidad
            CreateMap<LogroUpdateDto, Logro>()
                .ForMember(d => d.PuntosOtorgados, o => o.MapFrom(s => s.Puntos));

            // Logros realizados por socio: Entidad -> DTO
            CreateMap<SocioRealizaLogro, LogroUsuarioDto>()
                .ForMember(d => d.LogroId, o => o.MapFrom(s => s.LogroId))
                .ForMember(d => d.Nombre, o => o.MapFrom(s => s.Logro!.Nombre))
                .ForMember(d => d.PuntosOtorgados, o => o.MapFrom(s => s.PuntosOtorgados))
                .ForMember(d => d.FechaOtorgado, o => o.MapFrom(s => s.FechaOtorgado));
        }
    }
}
