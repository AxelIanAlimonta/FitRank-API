namespace FitRank_API.Domain.Entities
{
    public class ConfiguracionGrupoMuscular
    {
        public long Id { get; set; }

        public double MultiplicadorPeso { get; set; }
        public double MultiplicadorRepeticiones { get; set; }

        public double FactorProgresion { get; set; } = 1.0;

        public long GrupoMuscularId { get; set; }
        public GrupoMuscular GrupoMuscular { get; set; } = null!;
    }
}
