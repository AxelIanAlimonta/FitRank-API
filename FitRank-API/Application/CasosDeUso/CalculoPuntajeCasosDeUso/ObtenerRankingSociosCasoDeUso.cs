using FitRank_API.Application.DTOs.PuntajeDTOs;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.CalculoPuntajeCasosDeUso
{
    public class ObtenerRankingSociosCasoDeUso
    {
        private readonly ISocioRepositorio _socioRepositorio;


        public ObtenerRankingSociosCasoDeUso(ISocioRepositorio socioRepositorio)
        {
            _socioRepositorio = socioRepositorio;
        }

        public virtual async Task<List<SocioRankingDto>> Ejecutar(long gimnasioId, int cantidad)
        {
            return await _socioRepositorio.ObtenerRankingGeneralAsync(gimnasioId, cantidad);
        }
    }
}