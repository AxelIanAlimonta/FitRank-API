using AutoMapper;

namespace FitRank_API.Application.Mappings;

public class RutinaEjercicioProfile : Profile
{
    public RutinaEjercicioProfile()
    {
        CreateMap<Domain.Entities.RutinaEjercicio, Application.DTOs.RutinaEjercicioDTOs.ObtenerRutinaEjercicioDTO>().ReverseMap();
        CreateMap<Domain.Entities.RutinaEjercicio, Application.DTOs.RutinaEjercicioDTOs.AgregarRutinaEjercicioDTO>().ReverseMap();
        CreateMap<Domain.Entities.RutinaEjercicio, Application.DTOs.RutinaEjercicioDTOs.ActualizarRutinaEjercicioDTO>().ReverseMap();
    }
}
