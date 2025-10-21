using AutoMapper;

namespace FitRank_API.Application.Mappings;

public class LogroProfile : Profile
{
    public LogroProfile()
    {
        CreateMap<Domain.Entities.Logro, DTOs.LogroDTOs.ObtenerLogroDTO>().ReverseMap();
        CreateMap<Domain.Entities.Logro, DTOs.LogroDTOs.AgregarLogroDTO>().ReverseMap();
        CreateMap<Domain.Entities.Logro, DTOs.LogroDTOs.ActualizarLogroDTO>().ReverseMap();
    }
}
