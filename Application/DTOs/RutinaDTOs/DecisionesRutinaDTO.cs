namespace FitRank.API.Application.Rutinas.Abstractions
{
    public sealed class DecisionesRutinaDTO
    {
        // Seguridad / derivación
        public bool DerivarProfesional { get; set; }
        public HashSet<string> Precauciones { get; } = new();     // p.ej. PRECAUCION_HIPERTENSION
        public HashSet<string> Exclusiones { get; } = new();      // p.ej. EVITAR_CARGA_AXIAL

        // Modificadores
        public string Intensidad { get; set; } = "INTENSIDAD_MEDIA"; // BAJA | MEDIA | ALTA
        public string Volumen { get; set; } = "VOLUMEN_BASE";        // BAJO | BASE | ALTO
        public HashSet<string> Ajustes { get; } = new();             // p.ej. PRIORIDAD_TECNICA, VOLUMEN_CONSERVADOR

        // Objetivo
        public string Objetivo { get; set; } = "OBJETIVO_HIPERTROFIA";
        public string RangoReps { get; set; } = "REPETICIONES_6_12";
        public string Rir { get; set; } = "test";
        public string CardioApoyo { get; set; } = "CARDIO_APOYO_BAJO";

        // Split / sesión
        public string Division { get; set; } = "DIVISION_CUERPO_COMPLETO"; // FULL | UL | PPL (en español)
        public string Densidad { get; set; } = "DENSIDAD_NORMAL";           // ALTA | NORMAL
        public string TamanoSesion { get; set; } = "SESION_NORMAL";         // CORTA | LARGA | NORMAL

        // Equipo / preferencias
        public HashSet<string> EquipoPreferido { get; } = new();   // EQUIPO_MAQUINAS / EQUIPO_MANCUERNAS
        public bool IncluirCardio { get; set; }
        public bool UsuarioTieneListaEvitar { get; set; }

        // Todas las etiquetas crudas por si querés persistirlas o depurarlas
        public HashSet<string> Tags { get; } = new();
        // Mapa por workflow (para RulesExplainJson)
        public Dictionary<string, string[]> PorWorkflow { get; } = new();
    }
}
