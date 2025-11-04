using FitRank.API.Application.Rutinas.Abstractions;
using FitRank_API.Application.DTOs.RutinaDTOs;

namespace FitRank.API.Infrastructure.RulesEngineImpl
{
    // Input "plano" para reglas (evita enums y Math.* en JSON)
    public sealed class RulesInput
    {
        public int Edad { get; init; }
        public double PesoKg { get; init; }
        public int AlturaCm { get; init; }
        public double Imc { get; init; }                 // <- calculado acá
        public string Nivel { get; init; } = "";
        public int SesionesPorSemana { get; init; }
        public int MinutosPorSesion { get; init; }
        public string Objetivo { get; init; } = "";
        public int CalidadAlimentacion { get; init; }
        public int HorasSuenio { get; init; }
        public ScreeningDTO Screening { get; init; } = new();
        public PreferenciasDTO Preferencias { get; init; } = new();
    }

    public sealed class RoutineRulesRunner : IRoutineRulesRunner
    {
        private readonly IRulesEvaluator _evaluator;

        public RoutineRulesRunner(IRulesEvaluator evaluator) => _evaluator = evaluator;

        private static RulesInput ToRulesInput(RutinaRequestDTO d) => new()
        {
            Edad = d.Edad,
            PesoKg = d.PesoKg,
            AlturaCm = d.AlturaCm,
            Imc = d.AlturaCm > 0 ? d.PesoKg / Math.Pow(d.AlturaCm / 100.0, 2) : 0,
            Nivel = d.Nivel.ToString(),
            SesionesPorSemana = d.SesionesPorSemana,
            MinutosPorSesion = d.MinutosPorSesion,
            Objetivo = d.Objetivo.ToString(),
            CalidadAlimentacion = d.CalidadAlimentacion,
            HorasSuenio = d.HorasSuenio,
            Screening = d.Screening,
            Preferencias = d.Preferencias
        };

        private static IReadOnlyCollection<string> Get(
            IDictionary<string, IReadOnlyCollection<string>> map,
            params string[] aliases)
        {
            foreach (var a in aliases)
                if (map.TryGetValue(a, out var v)) return v;
            var hit = map.FirstOrDefault(kv => aliases.Any(a =>
                kv.Key.Equals(a, StringComparison.OrdinalIgnoreCase) ||
                kv.Key.StartsWith(a, StringComparison.OrdinalIgnoreCase)));
            return hit.Value ?? Array.Empty<string>();
        }

        public async Task<DecisionesRutinaDTO> RunAsync(object input)
        {
            var decisions = new DecisionesRutinaDTO();

            var rulesInput = input is RutinaRequestDTO dto ? ToRulesInput(dto) : (RulesInput)input;
            var porWf = await _evaluator.EvaluateAllAsync(rulesInput);
            foreach (var kv in porWf)
                decisions.PorWorkflow[kv.Key] = kv.Value.ToArray();

            void AddAll(IEnumerable<string> tags)
            {
                foreach (var t in tags)
                    decisions.Tags.Add(t);
            }

            // --- SALUD / SAFETY ---
            var safety = Get(porWf, "Salud", "safety");
            AddAll(safety);
            if (safety.Contains("DERIVAR_PROFESIONAL", StringComparer.OrdinalIgnoreCase)) decisions.DerivarProfesional = true;
            foreach (var t in safety)
            {
                if (t.StartsWith("PRECAUCION_", StringComparison.OrdinalIgnoreCase)) decisions.Precauciones.Add(t);
                if (t.StartsWith("EVITAR_", StringComparison.OrdinalIgnoreCase)) decisions.Exclusiones.Add(t);
            }

            // --- MODIFICADORES ---
            var modifiers = Get(porWf, "Modificadores", "modifiers");
            AddAll(modifiers);
            if (modifiers.Contains("INTENSIDAD_BAJA", StringComparer.OrdinalIgnoreCase)) decisions.Intensidad = "INTENSIDAD_BAJA";
            else if (modifiers.Contains("INTENSIDAD_ALTA", StringComparer.OrdinalIgnoreCase)) decisions.Intensidad = "INTENSIDAD_ALTA";
            else if (modifiers.Contains("INTENSIDAD_MEDIA", StringComparer.OrdinalIgnoreCase)) decisions.Intensidad = "INTENSIDAD_MEDIA";

            if (modifiers.Contains("VOLUMEN_BAJO", StringComparer.OrdinalIgnoreCase)) decisions.Volumen = "VOLUMEN_BAJO";
            else if (modifiers.Contains("VOLUMEN_ALTO", StringComparer.OrdinalIgnoreCase)) decisions.Volumen = "VOLUMEN_ALTO";
            else if (modifiers.Contains("VOLUMEN_BASE", StringComparer.OrdinalIgnoreCase)) decisions.Volumen = "VOLUMEN_BASE";

            if (modifiers.Contains("VOLUMEN_CONSERVADOR", StringComparer.OrdinalIgnoreCase)) decisions.Ajustes.Add("VOLUMEN_CONSERVADOR");
            if (modifiers.Contains("PRIORIDAD_TECNICA", StringComparer.OrdinalIgnoreCase)) decisions.Ajustes.Add("PRIORIDAD_TECNICA");
            if (modifiers.Contains("PREF_CARDIO_IMPACTO_BAJO", StringComparer.OrdinalIgnoreCase)) decisions.Ajustes.Add("PREF_CARDIO_IMPACTO_BAJO");
            if (modifiers.Contains("ENFOQUE_NUTRICION", StringComparer.OrdinalIgnoreCase)) decisions.Ajustes.Add("ENFOQUE_NUTRICION");

            // --- OBJETIVO ---
            var objective = Get(porWf, "Objetivo", "objective");
            AddAll(objective);
            if (objective.Contains("OBJETIVO_HIPERTROFIA", StringComparer.OrdinalIgnoreCase)) decisions.Objetivo = "OBJETIVO_HIPERTROFIA";
            else if (objective.Contains("OBJETIVO_FUERZA", StringComparer.OrdinalIgnoreCase)) decisions.Objetivo = "OBJETIVO_FUERZA";
            else if (objective.Contains("OBJETIVO_RESISTENCIA", StringComparer.OrdinalIgnoreCase)) decisions.Objetivo = "OBJETIVO_RESISTENCIA";
            else if (objective.Contains("OBJETIVO_PERDIDA_GRASA", StringComparer.OrdinalIgnoreCase)) decisions.Objetivo = "OBJETIVO_PERDIDA_GRASA";

            var reps = objective.FirstOrDefault(t => t.StartsWith("REPETICIONES_", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(reps)) decisions.RangoReps = reps!;
            var rir = objective.FirstOrDefault(t => t.StartsWith("RIR_", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(rir)) decisions.Rir = rir!;

            if (objective.Contains("CARDIO_APOYO_ALTO", StringComparer.OrdinalIgnoreCase)) decisions.CardioApoyo = "CARDIO_APOYO_ALTO";
            else if (objective.Contains("CARDIO_APOYO_MEDIO", StringComparer.OrdinalIgnoreCase)) decisions.CardioApoyo = "CARDIO_APOYO_MEDIO";
            else if (objective.Contains("CARDIO_APOYO_BAJO", StringComparer.OrdinalIgnoreCase)) decisions.CardioApoyo = "CARDIO_APOYO_BAJO";
            if (objective.Contains("CARDIO_APOYO_MAYOR", StringComparer.OrdinalIgnoreCase))
                decisions.CardioApoyo = decisions.CardioApoyo switch
                {
                    "CARDIO_APOYO_BAJO" => "CARDIO_APOYO_MEDIO",
                    "CARDIO_APOYO_MEDIO" => "CARDIO_APOYO_ALTO",
                    _ => "CARDIO_APOYO_ALTO"
                };

            // --- SPLIT ---
            var split = Get(porWf, "Split", "split", "Division", "División");
            AddAll(split);
            if (split.Contains("DIVISION_CUERPO_COMPLETO", StringComparer.OrdinalIgnoreCase)) decisions.Division = "DIVISION_CUERPO_COMPLETO";
            else if (split.Contains("DIVISION_SUPERIOR_INFERIOR", StringComparer.OrdinalIgnoreCase)) decisions.Division = "DIVISION_SUPERIOR_INFERIOR";
            else if (split.Contains("DIVISION_PPL", StringComparer.OrdinalIgnoreCase)) decisions.Division = "DIVISION_PPL";

            if (split.Contains("DENSIDAD_ALTA", StringComparer.OrdinalIgnoreCase)) decisions.Densidad = "DENSIDAD_ALTA";
            else if (split.Contains("DENSIDAD_NORMAL", StringComparer.OrdinalIgnoreCase)) decisions.Densidad = "DENSIDAD_NORMAL";

            if (split.Contains("SESION_CORTA", StringComparer.OrdinalIgnoreCase)) decisions.TamanoSesion = "SESION_CORTA";
            else if (split.Contains("SESION_LARGA", StringComparer.OrdinalIgnoreCase)) decisions.TamanoSesion = "SESION_LARGA";
            else decisions.TamanoSesion = "SESION_NORMAL";

            // --- EQUIPO / PREFERENCIAS ---
            var equipment = Get(porWf, "Preferencias", "equipment", "Equipo");
            AddAll(equipment);
            if (equipment.Contains("EQUIPO_MAQUINAS", StringComparer.OrdinalIgnoreCase)) decisions.EquipoPreferido.Add("EQUIPO_MAQUINAS");
            if (equipment.Contains("EQUIPO_MANCUERNAS", StringComparer.OrdinalIgnoreCase)) decisions.EquipoPreferido.Add("EQUIPO_MANCUERNAS");
            if (equipment.Contains("INCLUIR_CARDIO", StringComparer.OrdinalIgnoreCase)) decisions.IncluirCardio = true;
            if (equipment.Contains("LISTA_USUARIO_EVITAR", StringComparer.OrdinalIgnoreCase)) decisions.UsuarioTieneListaEvitar = true;

            return decisions;
        }
    }
}
