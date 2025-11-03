using FitRank_API.Application.DTOs.RankingDTOs;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IRankingRepositorio
    {
        Task<List<RankingDTO>> ObtenerTopSociosAsync(int top);
        Task<PosicionDTO> ObtenerPosicionPorIdAsync(long socioId);
    }
}
