using FitRank_API.Application.DTOs.Rankig;
using FitRank_API.Application.DTOs.Ranking;

namespace FitRank_API.Application.Interfaces
{
    public interface IRankingService
    {
        Task<List<MostrarRankingDTO>> MostrarRankingAsync();
        Task<List<MostrarRankingPorGrupoMuscular>> MostrarRankingPorGrupoMuscularAsync();
    }
}
