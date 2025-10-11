using FitRank_API.Domain.Enums;

namespace FitRank_API.Domain.Entities
{
    public class ConfiguracionGrupoMuscular
    {
        public int Id { get; set; }
        public GrupoMuscular GrupoMuscular { get; set; }
        public double MultiplicadorPeso { get; set; }
        public double MultiplicadorRepeticiones { get; set; }
    }
}
