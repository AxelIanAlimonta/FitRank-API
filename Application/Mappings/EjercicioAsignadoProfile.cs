using AutoMapper;

namespace FitRank_API.Application.Mappings;

public class EjercicioAsignadoProfile : Profile
{
    public EjercicioAsignadoProfile()
    {
        CreateMap<Domain.Entities.EjercicioAsignado, Application.DTOs.EjercicioAsignadoDTOs.ObtenerEjercicioAsignadoDTO>().ReverseMap();
        CreateMap<Domain.Entities.EjercicioAsignado, Application.DTOs.EjercicioAsignadoDTOs.AgregarEjercicioAsignadoDTO>().ReverseMap();
        CreateMap<Domain.Entities.EjercicioAsignado, Application.DTOs.EjercicioAsignadoDTOs.ActualizarEjercicioAsignadoDTO>().ReverseMap();
    }

}
