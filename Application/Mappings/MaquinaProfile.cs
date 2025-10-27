using AutoMapper;
using FitRank_API.Application.DTOs.MaquinaDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class MaquinaProfile : Profile
    {
        public MaquinaProfile()
        {
            CreateMap<Maquina, ObtenerMaquinaDTO>();

            CreateMap<AgregarMaquinaDTO, Maquina>();

            CreateMap<ActualizarMaquinaDTO, Maquina>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
                {
                    if (srcMember == null) return false;
                    return srcMember is string s ? !string.IsNullOrWhiteSpace(s) : true;
                }));
        }
    }
}
