using System;
using FitRank_API.Application.DTOs.BatallaDTOs;
using FitRank_API.Domain.Enums;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Application.UseCases.Batallas
{
    public class FinalizarBatallaCasoDeUso
    {
        private readonly FitRankDbContext _context;

        public FinalizarBatallaCasoDeUso(FitRankDbContext context)
        {
            _context = context;
        }

        public virtual async Task<ResultadoBatallaDTO> Ejecutar(long batallaId)
        {
            var batalla = await _context.Batallas
                .FirstOrDefaultAsync(b => b.Id == batallaId);

            if (batalla == null)
                throw new Exception($"La batalla con ID {batallaId} no existe.");

            if (batalla.Estado != BatallaEstado.Activa)
                throw new Exception("La batalla ya fue finalizada o no está activa.");
            if (batalla.Estado == BatallaEstado.Finalizada)
                throw new Exception("La batalla ya fue finalizada.");
            if (batalla.Estado == BatallaEstado.Rechazada)
                throw new Exception("La batalla está cancelada y no puede finalizarse.");

            var fechaFin = batalla.FechaFin ?? DateTime.Now;

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

            // Guardamos puntajes finales
            batalla.PuntosA = puntosA;
            batalla.PuntosB = puntosB;
            batalla.Estado = BatallaEstado.Finalizada;
            batalla.FechaFin = fechaFin;


            // Determinar ganador
            //string ganador;
            int? ganadorId = null;
            if (puntosA > puntosB) ganadorId = batalla.SocioAId;
            else if (puntosB > puntosA) ganadorId = batalla.SocioBId;

            batalla.GanadorId = ganadorId;

            await _context.SaveChangesAsync();
            return new ResultadoBatallaDTO
            {
                BatallaId = batalla.Id,
                PuntosJugadorA = puntosA,
                PuntosJugadorB = puntosB,
                Estado = batalla.Estado,
                GanadorId = ganadorId
            };
        }
    }
}
