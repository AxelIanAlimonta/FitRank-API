using AutoMapper;
using FitRank_API.Application.DTOs.MaquinaDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class MaquinaProfile : Profile
    {
        public MaquinaProfile()
        {
            CreateMap<Maquina, ObtenerMaquinaDTO>().ReverseMap();

            CreateMap<AgregarMaquinaDTO, Maquina>().ReverseMap();

            CreateMap<ActualizarMaquinaDTO, Maquina>().ReverseMap();
        }
    }
}
