using AutoMapper;
using FitRank_API.Application.DTOs.ProfesorDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class ProfesorProfile : Profile
    {
        public ProfesorProfile()
        {

            CreateMap<AgregarProfesorDTO, Profesor>()
           .ForMember(dest => dest.GimnasioId, opt => opt.MapFrom(src => src.GimnasioId)); 
            CreateMap<ActualizarProfesorDTO, Profesor>();
            CreateMap<Profesor, ProfesorDTO>()
           .ForMember(dest => dest.GimnasioNombre,
               opt => opt.MapFrom(src => src.Gimnasio != null ? src.Gimnasio.Nombre : null));
        }
    }
}
