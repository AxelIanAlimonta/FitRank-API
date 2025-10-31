using AutoMapper;
using FitRank_API.Application.DTOs.MedidaCorporalDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings;

public class MedidaCorporalProfile : Profile
{
    public MedidaCorporalProfile()
    {
        CreateMap<AgregarMedidaCorporalDTO, MedidaCorporal>().ReverseMap();
        CreateMap<ActualizarMedidaCorporalDTO, MedidaCorporal>().ReverseMap();
        CreateMap<MedidaCorporal, ObtenerMedidaCorporalDTO>().ReverseMap();
    }
}
