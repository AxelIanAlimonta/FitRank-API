using AutoMapper;
using FitRank_API.Application.DTOs.PuntajeDTOs;
using FitRank_API.Application.DTOs.RankingDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.CalculoPuntajeCasosDeUso
{
    public class ObtenerRankingPorFechaCasoDeUso
    {
        private readonly IMapper _mapper;
        private readonly ISocioRepositorio _socioRepositorio;
        public ObtenerRankingPorFechaCasoDeUso( ISocioRepositorio socioRepositorio ,IMapper mapper)
        {
            _socioRepositorio = socioRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<List<ObtenerRankingPorFechaDTO>> Ejecutar(long gimnasioId, int cantidad, DateOnly desde, DateOnly hasta)
        {
            var socios = await _socioRepositorio.ObtenerSociosParaRankingAsync(gimnasioId);
            foreach (var s in socios)
            {
                Console.WriteLine($"{s.Id} - {s.Nombre} {s.Apellido}");
            }
            var ranking = socios.Select(s =>
            {
                var actividades = s.Entrenamientos?
                .Where(e =>
                    DateOnly.FromDateTime(e.Fecha) >= desde &&
                    DateOnly.FromDateTime(e.Fecha) <= hasta)
                .SelectMany(e => e.Actividades ?? Enumerable.Empty<Actividad>())
                ?? Enumerable.Empty<Actividad>();

                double puntajeTotal = actividades.Sum(a => a.Punto ?? 0);

                return new ObtenerRankingPorFechaDTO
                {
                    SocioId = s.Id,
                    NombreCompleto = $"{s.Nombre} {s.Apellido}",
                    PuntajeTotal = puntajeTotal,
                    Desde = desde,
                    Hasta = hasta
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
