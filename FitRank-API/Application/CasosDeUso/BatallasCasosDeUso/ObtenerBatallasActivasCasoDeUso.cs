using FitRank_API.Application.DTOs.BatallaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Application.CasosDeUso.BatallasCasosDeUso
{
    public class ObtenerBatallasActivasCasoDeUso
    {
        private readonly FitRankDbContext _context;

        public ObtenerBatallasActivasCasoDeUso(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<List<HistorialBatallaDTO>> Ejecutar(int socioId)
        {
            var batallas = await _context.Batallas
                .Where(b => b.Estado == BatallaEstado.Activa &&
                            (b.SocioAId == socioId || b.SocioBId == socioId))
                .ToListAsync();

            var resultado = new List<HistorialBatallaDTO>();

            foreach (var b in batallas)
            {
                // Determinar quién es el otro jugador
                long oponenteId = b.SocioAId == socioId ? b.SocioBId : b.SocioAId;

                // Obtener nombre del oponente (si ya tenés entidad Socio)
                var oponente = await _context.Socios
                    .Where(s => s.Id == oponenteId)
                    .Select(s => s.Nombre + " " + s.Apellido)
                    .FirstOrDefaultAsync();

                resultado.Add(new HistorialBatallaDTO
                {
                    BatallaId = b.Id,
                    Estado = b.Estado,
                    FechaInicio = b.FechaInicio,
                    FechaFin = b.FechaFin,
                    PuntosA = b.PuntosA,
                    PuntosB = b.PuntosB,
                    Oponente = oponente ?? "Usuario",
                    UsuarioEsA = b.SocioAId == socioId
                });
            }

            return resultado;
        }
    }
}
