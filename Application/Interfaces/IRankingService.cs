using FitRank_API.Application.DTOs.Rankig;

namespace FitRank_API.Application.Interfaces
{
    public interface IRankingService
    {
        List<MostrarRankingDTO> CalcularRanking();
    }
}
