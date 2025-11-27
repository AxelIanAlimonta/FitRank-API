using FitRank_API.Application.DTOs.BatallaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;
using FitRank_API.Infrastructure.Persistence;

namespace FitRank_API.Application.CasosDeUso.BatallasCasosDeUso
{
    public class CrearBatallaCasoDeUso
    {
        private readonly FitRankDbContext _context;

        public CrearBatallaCasoDeUso(FitRankDbContext context)
        {
            _context = context;
        }

        public virtual async Task<BatallaPunto> Ejecutar(CrearBatallaDTO dto)
        {
            var batalla = new BatallaPunto
            {
                SocioAId = dto.SocioAId,
                SocioBId = dto.SocioBId,
                Tipo = dto.Tipo,
                FechaInicio = DateTime.UtcNow,
                FechaFin = DateTime.UtcNow.AddDays(dto.DiasDuracion),
                Estado = BatallaEstado.Pendiente
            };

            await _context.Batallas.AddAsync(batalla);
            await _context.SaveChangesAsync();

            return batalla;
        }
    }
}
