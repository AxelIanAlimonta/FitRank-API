using FitRank_API.Application.DTOs.Rankig;
using FitRank_API.Application.Interfaces;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.Services
{


    public class RankingServiceImpl : IRankingService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        public List<MostrarRankingDTO> CalcularRanking()
        {
            var usuarios = _usuarioRepository.GetUsuariosConPuntuaciones();
            return usuarios .Select(u => new MostrarRankingDTO
            {
                userName = u.Username,
                Nivel = u.Nivel,
                TotalPuntos = u.PuntuacionesDiarias.Sum(p => p.PuntosAsistencia + p.PuntosEjercicios)
            }).OrderByDescending(r => r.TotalPuntos).ToList();
        }
    }
}


