namespace FitRank_API.Domain.Entities
{
    public class ConfiguracionGrupoMuscular
    {
        public long Id { get; set; }
        public double Multiplicadopeso { get; set; }
        public double MultiplicadorRepeticiones { get; set; }

        public long GrupoMuscularId { get; set; }
        public GrupoMuscular GrupoMuscular { get; set; } = null!;
    }
}
