using System.Text.RegularExpressions;
using AutoMapper;
using FitRank_API.Application.DTOs.PuntajeDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.CalculoPuntajeCasosDeUso
{
    public class ObtenerRankingPorGrupoMuscularCasoDeUso
    {
        private readonly ISocioRepositorio _socioRepositorio;
        private readonly IMapper _mapper;

        public ObtenerRankingPorGrupoMuscularCasoDeUso(ISocioRepositorio socioRepositorio, IMapper mapper)
        {
            _socioRepositorio = socioRepositorio;
            _mapper = mapper;
        }

        public async Task<List<ObtenerRankingPorGrupoMuscularDTO>> Ejecutar(long gimnasioId, string grupo, int cantidad)
        {
            var socios = await _socioRepositorio.ObtenerSociosParaRankingAsync(gimnasioId);
            foreach (var s in socios)
            {
                Console.WriteLine($"{s.Id} - {s.Nombre} {s.Apellido}");
            }
            var ranking = socios.Select(s =>
            {
                var actividades = s.Entrenamientos?
                    .SelectMany(e => e.Actividades ?? Enumerable.Empty<Actividad>())
                        .Where(a => 
                        a.Serie?.EjercicioAsignado?.Ejercicio?.GrupoMuscular != null &&
                    a.Serie.EjercicioAsignado.Ejercicio.GrupoMuscular.Nombre
                        .Equals(grupo, StringComparison.OrdinalIgnoreCase))
                ?? Enumerable.Empty<Actividad>();

                double puntajeGrupo = actividades.Sum(a => a.Punto ?? 0);

                return new ObtenerRankingPorGrupoMuscularDTO
                {
                    SocioId = s.Id,
                    NombreCompleto = $"{s.Nombre} {s.Apellido}",
                    GrupoMuscular = grupo,
                    PuntajeTotal = puntajeGrupo
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
