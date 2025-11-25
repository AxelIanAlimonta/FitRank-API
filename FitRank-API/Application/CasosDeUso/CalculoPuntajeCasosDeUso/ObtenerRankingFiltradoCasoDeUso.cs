using AutoMapper;
using FitRank_API.Application.DTOs.RankingDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Application.DTOs.PuntajeDTOs;
using FitRank_API.Application.DTOs.RankingDTOs;

namespace FitRank_API.Application.CasosDeUso.CalculoPuntajeCasosDeUso
{
    public class ObtenerRankingFiltradoCasoDeUso
    {
        private readonly ISocioRepositorio _socioRepositorio;
        private readonly IGrupoMuscularRepositorio _grupoMuscularRepositorio;
        private readonly IMapper _mapper;

        public ObtenerRankingFiltradoCasoDeUso(
            ISocioRepositorio socioRepositorio,
            IMapper mapper,
            IGrupoMuscularRepositorio grupoMuscularRepositorio)
        {
            _socioRepositorio = socioRepositorio;
            _mapper = mapper;
            _grupoMuscularRepositorio = grupoMuscularRepositorio;
        }

        public async Task<List<SocioRankingDto>> Ejecutar(
            long gimnasioId,
            long? grupoId,
            DateOnly? desde,
            DateOnly? hasta,
            int cantidad)
        {
            var socios = await _socioRepositorio.ObtenerSociosParaRankingAsync(gimnasioId);

            // Si hay grupo muscular, validar existencia
            GrupoMuscular? grupo = null;
            if (grupoId.HasValue && grupoId > 0)
            {
                grupo = await _grupoMuscularRepositorio.ObtenerPorIdAsync(grupoId.Value);
                if (grupo == null)
                    throw new ArgumentException("El grupo muscular no existe.");
            }

            var ranking = socios.Select(s =>
            {
                var entrenamientos = s.Entrenamientos?.AsEnumerable() ?? Enumerable.Empty<Entrenamiento>();

                // FILTRO POR FECHAS (si se enviaron)
                if (desde.HasValue)
                {
                    entrenamientos = entrenamientos.Where(e =>
                        DateOnly.FromDateTime(e.Fecha) >= desde.Value);
                }

                if (hasta.HasValue)
                {
                    entrenamientos = entrenamientos.Where(e =>
                        DateOnly.FromDateTime(e.Fecha) <= hasta.Value);
                }


                // Expandir actividades
                var actividades = entrenamientos
                    .SelectMany(e => e.Actividades ?? Enumerable.Empty<Actividad>());

                // FILTRO POR GRUPO MUSCULAR (si se envió)
                if (grupoId.HasValue && grupoId > 0)
                {
                    actividades = actividades.Where(a =>
                        a.Serie?.EjercicioAsignado?.Ejercicio?.GrupoMuscularId == grupoId.Value);
                }

                double puntajeTotal = actividades.Sum(a => a.Punto ?? 0);

                return new SocioRankingDto
                {
                    SocioId = s.Id,
                    NombreCompleto = $"{s.Nombre} {s.Apellido}",
                    PuntajeTotal = puntajeTotal
                };
            })
            .Where(s => s.PuntajeTotal > 0)
            .OrderByDescending(s => s.PuntajeTotal)
            .Take(cantidad)
            .ToList();

            return ranking;
        }
    }
}
