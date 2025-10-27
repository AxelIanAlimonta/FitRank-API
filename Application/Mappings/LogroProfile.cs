using AutoMapper;
using FitRank_API.Application.DTOs.LogroDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings;

public class LogroProfile : Profile
{
    public LogroProfile()
    {
        CreateMap<Domain.Entities.Logro, DTOs.LogroDTOs.ObtenerLogroDTO>().ReverseMap();
        CreateMap<Domain.Entities.Logro, DTOs.LogroDTOs.AgregarLogroDTO>().ReverseMap();

        // Mapeo para ActualizarLogroDTO a Logro, ignorando atributos vacíos/nulos
        CreateMap<ActualizarLogroDTO, Logro>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
            {
                if (srcMember == null) return false;
                return srcMember is string s ? !string.IsNullOrWhiteSpace(s) : true;
            }));
    }
}
