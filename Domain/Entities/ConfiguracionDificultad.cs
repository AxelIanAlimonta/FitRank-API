using FitRank_API.Domain.Enums;

namespace FitRank_API.Domain.Entities
{
    public class ConfiguracionDificultad
    {
        public int Id { get; set; }
        public Dificultad Dificultad { get; set; }
        public double Multiplicador { get; set; }
    }
}
