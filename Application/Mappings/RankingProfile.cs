using AutoMapper;
using FitRank_API.Application.DTOs.Rankig;
using FitRank_API.Application.DTOs.Ranking;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Mappings
{
    public class RankingProfile: Profile
    {
        public RankingProfile() {

            CreateMap<Ranking, MostrarRankingDTO>().ReverseMap();
            CreateMap<Ranking, MostrarRankingPorGrupoMuscular>().ReverseMap();


        }
        
    }
}
