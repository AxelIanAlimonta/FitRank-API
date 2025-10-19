using AutoMapper;

namespace FitRank_API.Application.Mappings;

public class SerieAsignadaProfile : Profile
{
    public SerieAsignadaProfile()
    {
        CreateMap<Domain.Entities.SerieAsignada, DTOs.SerieAsignadaDTOs.ObtenerSerieAsignadaDTO>();
        CreateMap<DTOs.SerieAsignadaDTOs.AgregarSerieAsignadaDTO, Domain.Entities.SerieAsignada>();
        CreateMap<DTOs.SerieAsignadaDTOs.ActualizarSerieAsignadaDTO, Domain.Entities.SerieAsignada>();
    }
}
