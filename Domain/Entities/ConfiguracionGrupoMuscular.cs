namespace FitRank_API.Domain.Entities
{
    public class ConfiguracionGrupoMuscular
    {
        public long Id { get; set; }
        public long GrupoMuscularId { get; set; } // FK
        public GrupoMuscular GrupoMuscular { get; set; } // Navegación
        public double Multiplicadopeso { get; set; }
        public double MultiplicadorRepeticiones { get; set; }
    }
}
