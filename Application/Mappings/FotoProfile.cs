using AutoMapper;
using FitRank_API.Application.DTOs.FotoDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class FotoProfile : Profile
    {
        public FotoProfile()
        {
            CreateMap<Foto, ObtenerFotoDTO>().ReverseMap();
            CreateMap<AgregarFotoDTO, Foto>();
            CreateMap<ActualizarFotoDTO, Foto>();
        }
    }
}

