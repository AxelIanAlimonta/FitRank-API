using FitRank_API.Application.DTOs.BatallaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Application.CasosDeUso.BatallasCasosDeUso
{
    public class ObtenerProgresoBatallaCasoDeUso
    {
        private readonly FitRankDbContext _context;

        public ObtenerProgresoBatallaCasoDeUso(FitRankDbContext context)
        {
            _context = context;
        }

        public virtual async Task<ProgresoBatallaDTO?> Ejecutar(int batallaId)
        {
            var batalla = await _context.Batallas.FindAsync(batallaId);
            if (batalla == null) return null;

            DateTime fechaFin = batalla.Estado == BatallaEstado.Finalizada && batalla.FechaFin.HasValue
                ? batalla.FechaFin.Value
                : DateTime.Now;


            var puntosA = await _context.Actividades
                .Where(a => a.Entrenamiento.SocioId == batalla.SocioAId &&
                            a.Entrenamiento.Fecha >= batalla.FechaInicio &&
                            a.Entrenamiento.Fecha <= fechaFin)
                .SumAsync(a => (double?)a.Punto) ?? 0;

            var puntosB = await _context.Actividades
                .Where(a => a.Entrenamiento.SocioId == batalla.SocioBId &&
                            a.Entrenamiento.Fecha >= batalla.FechaInicio &&
                            a.Entrenamiento.Fecha <= fechaFin)
                .SumAsync(a => (double?)a.Punto) ?? 0;


            return new ProgresoBatallaDTO
            {
                BatallaId = batalla.Id,
                PuntosJugadorA = puntosA,
                PuntosJugadorB = puntosB,
                FechaInicio = batalla.FechaInicio,
                FechaFin = batalla.FechaFin,
                PuntosGuardadosA = batalla.PuntosA, // para debug
                PuntosGuardadosB = batalla.PuntosB,
                ganadorId = batalla.GanadorId,
                UsuarioA = batalla.SocioAId,
                UsuarioB = batalla.SocioBId,
                Estado = batalla.Estado
            };
        }
    }
}
