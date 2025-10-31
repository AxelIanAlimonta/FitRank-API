using AutoMapper;
using FitRank_API.Application.DTOs.ProfesorDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class ProfesorProfile : Profile
    {
        public ProfesorProfile()
        {
            CreateMap<Profesor, ProfesorDTO>().ReverseMap();
            CreateMap<AgregarProfesorDTO, Profesor>();
            CreateMap<ActualizarProfesorDTO, Profesor>();
        }
    }
}
