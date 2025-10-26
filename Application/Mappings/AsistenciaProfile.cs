using AutoMapper;
using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class AsistenciaProfile : Profile
    {
        public AsistenciaProfile()
        {
            // DTO → Entidad
            CreateMap<AgregarAsistenciaDTO, Asistencia>();

            // Entidad → ResponseDTO
            CreateMap<Asistencia, AsistenciaResponseDTO>()
                .ForMember(dest => dest.AsistenciaId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.HoraEntrada, opt => opt.MapFrom(src => src.HoraEntrada));
        }
    }
}
