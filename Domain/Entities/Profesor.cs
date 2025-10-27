namespace FitRank_API.Domain.Entities
{
    public class Profesor : Usuario
    {
        public string Matricula { get; set; } = string.Empty;
        public double Sueldo { get; set; }
    }
}
