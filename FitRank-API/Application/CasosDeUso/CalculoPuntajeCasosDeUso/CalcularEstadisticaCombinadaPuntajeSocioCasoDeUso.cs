using FitRank_API.Application.DTOs.CalcularPuntajeDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FitRank_API.Application.UseCases
{
    public class CalcularEstadisticaCombinadaPuntajeSocioCasoDeUso
    {
        private readonly ISocioRepositorio _socioRepositorio;
        private readonly IActividadRepositorio _actividadRepositorio;

        public CalcularEstadisticaCombinadaPuntajeSocioCasoDeUso(
            ISocioRepositorio socioRepositorio,
            IActividadRepositorio actividadRepositorio)
        {
            _socioRepositorio = socioRepositorio;
            _actividadRepositorio = actividadRepositorio;
        }

        public virtual async Task<PuntajeTotalDTO?> Ejecutar(long socioId)
        {
            var socio = await _socioRepositorio.ObtenerSocioConEntrenamientosAsync(socioId);

            if (socio == null || socio.Entrenamientos == null || !socio.Entrenamientos.Any())
                return null;

            var actividades = socio.Entrenamientos
                .SelectMany(e => e.Actividades ?? Enumerable.Empty<Domain.Entities.Actividad>())
                .ToList();

            if (!actividades.Any())
                return null;

            double puntajeTotal = actividades.Sum(a => a.Punto ?? 0);

            var puntajePorGrupo = actividades
                .Where(a => a.EjercicioAsignado != null)
                .GroupBy(a => a.EjercicioAsignado.Ejercicio.GrupoMuscularId)
                .Select(g => new PuntajePorGrupoDTO
                {
                    GrupoMuscularId = g.Key,
                    Puntaje = g.Sum(a => a.Punto ?? 0)
                })
                .ToList();

            return new PuntajeTotalDTO
            {
                SocioId = socio.Id,
                PuntajeTotal = puntajeTotal,
                PuntajePorGrupo = puntajePorGrupo
            };
        }
    }
}
