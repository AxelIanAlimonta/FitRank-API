using AutoMapper;
using FitRank_API.Application.DTOs.GimnasioDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings;

public class GimnasioProfile : Profile
{
    public GimnasioProfile()
    {
        CreateMap<Domain.Entities.Gimnasio, DTOs.GimnasioDTOs.ObtenerGimnasioDTO>().ReverseMap();
        CreateMap<Domain.Entities.Gimnasio, DTOs.GimnasioDTOs.AgregarGimnasioDTO>().ReverseMap();

        // Mapeo para ActualizarGimnasioDTO a Gimnasio, ignorando atributos vacíos/nulos
        CreateMap<ActualizarGimnasioDTO, Gimnasio>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
            {
                if (srcMember == null) return false;
                return srcMember is string s ? !string.IsNullOrWhiteSpace(s) : true;
            }));
    }
}
