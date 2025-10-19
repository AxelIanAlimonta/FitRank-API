using AutoMapper;

namespace FitRank_API.Application.Mappings;

public class EjercicioProfile : Profile
{
    public EjercicioProfile()
    {
        CreateMap<Domain.Entities.Ejercicio, DTOs.EjercicioDTOs.EjercicioDTO>().ReverseMap();
        CreateMap<Domain.Entities.Ejercicio, DTOs.EjercicioDTOs.AgregarEjercicioDTO>().ReverseMap();
    }

}
