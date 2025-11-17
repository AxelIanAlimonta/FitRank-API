using FitRank_API.Application.DTOs.RankingDTOs;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IRankingRepositorio
    {
        //Usar los metodos de puntajeRepositorio
        Task<List<RankingDTO>> ObtenerTopSociosAsync(int top);
        Task<PosicionDTO> ObtenerPosicionPorIdAsync(long socioId);
    }
}
