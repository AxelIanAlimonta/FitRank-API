using AutoMapper;
using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class AsistenciaProfile : Profile
    {
        public AsistenciaProfile()
        {
       
            CreateMap<AgregarAsistenciaDTO, Asistencia>();

            CreateMap<Asistencia, AsistenciaDetalleUsuarioDTO>()
                .ForMember(dest => dest.GimnasioNombre, opt => opt.MapFrom(src => src.Gimnasio.Nombre));
           
            CreateMap<Asistencia, AsistenciaResponseDTO>()
                .ForMember(dest => dest.AsistenciaId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.HoraEntrada, opt => opt.MapFrom(src => src.HoraEntrada));

            CreateMap<AgregarAsistenciaDTO, Asistencia>();
            CreateMap<Asistencia, AsistenciaListadoDTO>()
                   .ForMember(dest => dest.NombreSocio,
                       opt => opt.MapFrom(src => $"{src.Usuario.Nombre} {src.Usuario.Apellido}"))
                   .ForMember(dest => dest.GimnasioNombre,
                       opt => opt.MapFrom(src => (src.Usuario as Socio)!.Gimnasio.Nombre));

        }
    }
}
