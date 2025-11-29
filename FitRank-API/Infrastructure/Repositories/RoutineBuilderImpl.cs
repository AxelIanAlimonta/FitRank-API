using FitRank.API.Application.Rutinas.Abstractions;
using FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Application.DTOs.SesionDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

[ExcludeFromCodeCoverage]
public sealed class RoutineBuilderImpl : IRoutineBuilder
{
    private readonly IEjercicioCatalogo _catalogo;

    public RoutineBuilderImpl(IEjercicioCatalogo catalogo)
    {
        _catalogo = catalogo;
    }

    public async Task<RutinaGeneradaPorIADTO> BuildAsync(
        RutinaRequestDTO input,
        DecisionesRutinaDTO d)
    {
        var seriesPorGrupo = d.Volumen switch
        {
            "VOLUMEN_BAJO" => 6,
            "VOLUMEN_BASE" => 10,
            "VOLUMEN_ALTO" => 14,
            _ => 10
        };

        var (repsMin, repsMax) = d.RangoReps switch
        {
            "REPETICIONES_3_6" => (3, 6),
            "REPETICIONES_6_12" => (6, 12),
            "REPETICIONES_8_15" => (8, 15),
            "REPETICIONES_12_20" => (12, 20),
            _ => (6, 12)
        };

        var rir = d.Rir switch
        {
            "RIR_0_2" => 1,
            "RIR_1_2" or "RIR_1_3" => 2,
            "RIR_2_3" => 3,
            _ => 2
        };

        int maxEjerciciosPorSesion = d.TamanoSesion switch
        {
            "SESION_CORTA" => 6,
            "SESION_LARGA" => 10,
            _ => 8 
        };

        string[] prioridad = { "Piernas", "Pecho", "Espalda", "Hombros", "Biceps", "Triceps", "Core" };

        var sesiones = Math.Clamp(input.SesionesPorSemana, 2, 6);
        var plan = GetSplitPlan(d.Division, sesiones);

        var dolores = MapExclusionesASitiosDolor(d.Exclusiones);

        var sesionesOut = new List<SesionIADTO>();
        var rng = new Random();

        var targetSeriesSemanal = d.Volumen switch
        {
            "VOLUMEN_BAJO" => 8,
            "VOLUMEN_BASE" => 12,
            "VOLUMEN_ALTO" => 16,
            _ => 12
        };

        var apariciones = plan
            .SelectMany(x => x)
            .GroupBy(g => g)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);

        var ultimoDiaUsado = new Dictionary<long, int>();

        for (int dia = 0; dia < sesiones; dia++)
        {
            var gruposDia = plan[dia];

            

            var candidatos = await _catalogo.BuscarAsync(
                new CatalogoQuery(
                    Grupos: gruposDia,
                    EquiposPreferidos: d.EquipoPreferido,
                    EvitarUsuario: Array.Empty<string>(),
                    Dolores: dolores
                ));

            if (input.Preferencias?.EjerciciosExcluidos?.Count > 0)
            {
                var nombresExcluidos = new HashSet<string>(
                    input.Preferencias.EjerciciosExcluidos,
                    StringComparer.OrdinalIgnoreCase
                );

                candidatos = candidatos
                    .Where(e => !e.Tags.Any(t => nombresExcluidos.Contains(t)) && !nombresExcluidos.Contains(e.Nombre))
                    .ToList();
            }


            if (candidatos.Count == 0 && d.EquipoPreferido is { Count: > 0 })
            {
                candidatos = await _catalogo.BuscarAsync(
                    new CatalogoQuery(
                        Grupos: gruposDia,
                        EquiposPreferidos: Array.Empty<string>(),
                        EvitarUsuario: input.Preferencias?.EjerciciosExcluidos?.ToArray() ?? Array.Empty<string>(),
                        Dolores: dolores
                    ));
            }

            var ejerciciosDia = new List<EjercicioAsignadoIADTO>();
            var usadosHoy = new HashSet<long>();

            foreach (var grupo in gruposDia.OrderBy(g => Array.IndexOf(prioridad, prioridad.FirstOrDefault(p => p.Equals(g, StringComparison.OrdinalIgnoreCase)))))
            {
                if (ejerciciosDia.Count >= maxEjerciciosPorSesion) break;

                var vecesSemana = apariciones.TryGetValue(grupo, out var v) ? Math.Max(1, v) : 1;
                var seriesGrupoSesion = (int)Math.Round((double)targetSeriesSemanal / vecesSemana);

                seriesGrupoSesion = Math.Clamp(seriesGrupoSesion, 3, 6);

                var delGrupo = candidatos
                    .Where(e => e.Tipo.Equals(grupo, StringComparison.OrdinalIgnoreCase)
                             && !usadosHoy.Contains(e.Id)
                             && (!ultimoDiaUsado.TryGetValue(e.Id, out var ultimo) || (dia - ultimo) >= 2)
                             && !e.ContraIndicaciones.Any(ci => dolores.Contains(ci, StringComparer.OrdinalIgnoreCase))
                           )
                    .ToList();

                if (delGrupo.Count == 0) continue;

                int ejerciciosAUsar = seriesGrupoSesion <= 4 ? 1 : 2;
                if (ejerciciosDia.Count + ejerciciosAUsar > maxEjerciciosPorSesion)
                    ejerciciosAUsar = Math.Max(1, maxEjerciciosPorSesion - ejerciciosDia.Count);

                var preferEquipo = new HashSet<string>(
                    d.EquipoPreferido ?? Enumerable.Empty<string>(),
                    StringComparer.OrdinalIgnoreCase
                );
                var elegidos = delGrupo
                    .OrderByDescending(e => e.Tags.Any(t => t.Equals("Multiarticular", StringComparison.OrdinalIgnoreCase)))
                    .ThenByDescending(e => preferEquipo.Count == 0 || preferEquipo.Contains($"EQUIPO_{e.EquipoNecesario}".ToUpperInvariant()))
                    .ThenBy(_ => rng.Next())
                    .Take(ejerciciosAUsar)
                    .ToList();

                if (elegidos.Count == 0) continue;

                int s1 = ejerciciosAUsar == 1 ? seriesGrupoSesion : (int)Math.Round(seriesGrupoSesion * 0.6);
                int s2 = Math.Max(0, seriesGrupoSesion - s1);

                foreach (var (ej, idx) in elegidos.Select((e, i) => (e, i)))
                {
                    var sets = idx == 0 ? s1 : s2;
                    if (sets <= 0) continue;

                    sets = Math.Clamp(sets, 3, 4);

                    usadosHoy.Add(ej.Id);

                    var pesoRecomendado = CalcularPesoRecomendado(input, ej.Tipo);


                    var baseReps = rng.Next(repsMin, repsMax + 1);
                    var series = Enumerable.Range(1, sets)
                        .Select(n =>
                        {
                            var delta = (n == sets && baseReps > repsMin) ? -1 : 0;
                            return new SerieAsignadaIADTO(
                                Nro: n,
                                Reps: Math.Clamp(baseReps + delta, repsMin, repsMax),
                                Rir: rir,
                                PesoObjetivo: pesoRecomendado
                            );
                        }).ToList();

                    ejerciciosDia.Add(new EjercicioAsignadoIADTO(
                        EjercicioId: ej.Id,
                        Nombre: ej.Nombre,
                        Tipo: grupo,
                        Equipo: ej.EquipoNecesario.ToString(),
                        Series: series,
                        Tags: ej.Tags,
                        ContraIndicaciones: ej.ContraIndicaciones
                    ));
                    ultimoDiaUsado[ej.Id] = dia;

                    if (ejerciciosDia.Count >= maxEjerciciosPorSesion) break;
                }

                if (ejerciciosDia.Count >= maxEjerciciosPorSesion) break;
            }

            CardioIADTO? cardio = null;
            if (d.IncluirCardio || d.CardioApoyo is "CARDIO_APOYO_MEDIO" or "CARDIO_APOYO_ALTO")
            {
                var min = d.CardioApoyo switch
                {
                    "CARDIO_APOYO_ALTO" => 25,
                    "CARDIO_APOYO_MEDIO" => 15,
                    _ => input.MinutosPorSesion < 45 ? 10 : 12
                };

                var intensidad = d.Intensidad switch
                {
                    "INTENSIDAD_ALTA" => "Moderada",
                    "INTENSIDAD_MEDIA" => "Ligera/Moderada",
                    _ => "Ligera"
                };

                cardio = new CardioIADTO("Bicicleta estática", min, intensidad);
            }


            const int MIN_EJERCICIOS_SESION = 5;

            if (ejerciciosDia.Count < MIN_EJERCICIOS_SESION)
            {
                var pool = candidatos
                    .Where(e => !usadosHoy.Contains(e.Id))
                    .OrderByDescending(e => e.Tags.Any(t => t.Equals("Multiarticular", StringComparison.OrdinalIgnoreCase)))
                    .ThenBy(_ => rng.Next())
                    .ToList();

                foreach (var ej in pool)
                {
                    if (ejerciciosDia.Count >= MIN_EJERCICIOS_SESION) break;

                    var baseReps = rng.Next(repsMin, repsMax + 1);
                    var series = Enumerable.Range(1, 3)
                        .Select(n => new SerieAsignadaIADTO(n, baseReps, rir, CalcularPesoRecomendado(input, ej.Tipo)))
                        .ToList();

                    ejerciciosDia.Add(new EjercicioAsignadoIADTO(
                        EjercicioId: ej.Id,
                        Nombre: ej.Nombre,
                        Tipo: ej.Tipo,
                        Equipo: ej.EquipoNecesario.ToString(),
                        Series: series,
                        Tags: ej.Tags,
                        ContraIndicaciones: ej.ContraIndicaciones
                    ));

                    usadosHoy.Add(ej.Id);
                }
            }

            var nombreDia = d.Division switch
            {
                "DIVISION_SUPERIOR_INFERIOR" => dia % 2 == 0 ? "Superior" : "Inferior",
                "DIVISION_PPL" => new[] { "Pull", "Push", "Legs", "Push", "Pull", "Legs" }[dia % 6],
                _ => $"Full Body {dia + 1}"
            };

            sesionesOut.Add(new SesionIADTO(
                Nombre: nombreDia,
                Ejercicios: ejerciciosDia,
                Cardio: cardio
            ));

        }

        var nombreRutina = $"{SanitizeName(d.Objetivo)} · {SanitizeName(d.Division)}";

        return new RutinaGeneradaPorIADTO(
            Nombre: nombreRutina,
            Objetivo: d.Objetivo,
            Division: d.Division,
            Sesiones: sesiones,
            MinutosPorSesion: input.MinutosPorSesion,
            SesionesPlan: sesionesOut,
            InputSnapshot: input,                   
            RulesExplain: new { decisiones = d }  
        );
    }

    private static List<List<string>> GetSplitPlan(string? division, int sesiones)
    {
        division = (division ?? "DIVISION_CUERPO_COMPLETO").ToUpperInvariant();

        var FB = new List<string> { "Pecho", "Espalda", "Piernas", "Hombros", "Biceps", "Triceps", "Core" };

        if (division == "DIVISION_SUPERIOR_INFERIOR")
        {
            var sup = new List<string> { "Pecho", "Espalda", "Hombros", "Biceps", "Triceps", "Core" };
            var inf = new List<string> { "Piernas", "Core" };
            return Enumerable.Range(0, sesiones)
                .Select(i => i % 2 == 0 ? sup : inf)
                .ToList();
        }

        if (division == "DIVISION_PPL")
        {
            var pull = new List<string> { "Espalda", "Biceps" }; 
            var push = new List<string> { "Pecho", "Hombros", "Triceps" }; 
            var legs = new List<string> { "Piernas", "Core" };
            var seq = new[] { pull, push, legs, push, pull, legs };
            return Enumerable.Range(0, sesiones).Select(i => seq[i % 6]).ToList();
        }

        // full body (default)
        return Enumerable.Range(0, sesiones).Select(_ => FB).ToList();
    }

    private static List<string> MapExclusionesASitiosDolor(ICollection<string> exclusiones)
    {
        var dolores = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var x in exclusiones ?? Array.Empty<string>())
        {
            if (x.Contains("HOMBRO", StringComparison.OrdinalIgnoreCase)) dolores.Add("Hombro");
            if (x.Contains("RODILLA", StringComparison.OrdinalIgnoreCase)) dolores.Add("Rodilla");
            if (x.Contains("LUMBAR", StringComparison.OrdinalIgnoreCase)) dolores.Add("Lumbar");
        }
        return dolores.ToList();
    }


    private static double CalcularPesoRecomendado(RutinaRequestDTO input, string tipoEjercicio)
    {
        
        double pesoUsuario = input.PesoKg > 0 ? input.PesoKg : 70; 

        // Ajuste según grupo muscular
        double factor = tipoEjercicio switch
        {
            "Piernas" => 0.7,
            "Espalda" => 0.6,
            "Pecho" => 0.5,
            "Hombros" => 0.4,
            "Biceps" => 0.3,
            "Triceps" => 0.3,
            "Core" => 0.2,
            _ => 0.4
        };

        return Math.Round(pesoUsuario * factor, 1); 
    }


    private static string SanitizeName(string raw)
        => raw.Replace("OBJETIVO_", "", StringComparison.OrdinalIgnoreCase)
              .Replace("DIVISION_", "", StringComparison.OrdinalIgnoreCase)
              .Replace('_', ' ');
}
