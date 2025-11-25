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
        private readonly IGrupoMuscularRepositorio _grupoMuscularRepositorio;
        private readonly IMapper _mapper;

        public ObtenerRankingPorGrupoMuscularCasoDeUso(ISocioRepositorio socioRepositorio, IMapper mapper, IGrupoMuscularRepositorio grupoMuscularRepositorio)
        {
            _socioRepositorio = socioRepositorio;
            _mapper = mapper;
            _grupoMuscularRepositorio = grupoMuscularRepositorio;
        }

        public virtual async Task<List<ObtenerRankingPorGrupoMuscularDTO>> Ejecutar(long gimnasioId, string grupo, int cantidad)
        {
            var grupoMuscular = await _grupoMuscularRepositorio.ObtenerPorIdAsync(grupoId);
            if (grupoMuscular == null)
            {
                throw new ArgumentException("El grupo muscular no existe.");
            }

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
                        a.Serie?.EjercicioAsignado?.Ejercicio?.GrupoMuscularId == grupoId
                        )
                ?? Enumerable.Empty<Actividad>();

                double puntajeGrupo = actividades.Sum(a => a.Punto ?? 0);

                return new ObtenerRankingPorGrupoMuscularDTO
                {
                    SocioId = s.Id,
                    NombreCompleto = $"{s.Nombre} {s.Apellido}",
                    GrupoMuscular = grupoMuscular.Nombre,
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
