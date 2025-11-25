using AutoMapper;
using FitRank_API.Application.DTOs.ReporteDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class ReporteProfile : Profile
    {
        public ReporteProfile()
        {
            // DTO → Entidad
            CreateMap<AgregarReporteDTO, Reporte>()
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(_ => true));

            CreateMap<ActualizarReporteDTO, Reporte>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario, opt => opt.Ignore())
                .ForMember(dest => dest.Gimnasio, opt => opt.Ignore());

            // Entidad → DTO
            CreateMap<Reporte, ReporteDTO>();
        }
    }
}
