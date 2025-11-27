using FitRank_API.Application.DTOs.RankingDTOs;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class RankingRepositorioImpl : IRankingRepositorio
    {
        private readonly FitRankDbContext _context;

        public RankingRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<List<RankingDTO>> ObtenerTopSociosAsync(int top)
        {
            var ranking = await (from socio in _context.Socios
                                 join puntaje in _context.Puntajes on socio.Id equals puntaje.SocioId
                                 group puntaje by new { socio.Id, socio.Nombre, socio.Apellido } into g
                                 orderby g.Sum(p => p.Valor) descending
                                 select new RankingDTO
                                 {
                                     SocioId = g.Key.Id,
                                     NombreCompleto = g.Key.Nombre + " " + g.Key.Apellido,
                                     PuntajeTotal = g.Sum(p => p.Valor)
                                 })
                                 .Take(top)
                                 .ToListAsync();
            return ranking;
        }

        public async Task<PosicionDTO?> ObtenerPosicionPorIdAsync(long socioId)
        {
            var ranking = await (from socio in _context.Socios
                                 join puntaje in _context.Puntajes on socio.Id equals puntaje.SocioId
                                 group puntaje by new { socio.Id, socio.Nombre, socio.Apellido } into g
                                 orderby g.Sum(p => p.Valor) descending
                                 select new
                                 {
                                     SocioId = g.Key.Id,
                                     NombreCompleto = g.Key.Nombre + " " + g.Key.Apellido,
                                     PuntajeTotal = g.Sum(p => p.Valor)
                                 })
                                 .ToListAsync();
            var posicion = ranking
                .Select((r, index) => new PosicionDTO
                {
                    Posicion = index + 1,
                    SocioId = r.SocioId,
                    NombreCompleto = r.NombreCompleto,
                    PuntajeTotal = r.PuntajeTotal
                })
                .FirstOrDefault(r => r.SocioId == socioId);
            return posicion;
        }
    }
}
