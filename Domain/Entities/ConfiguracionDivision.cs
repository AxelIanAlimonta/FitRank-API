namespace FitRank_API.Domain.Entities
{
    public class ConfiguracionDivision
    {
        public int Id { get; set; }
        public string Nombre { get; set; } // Bronce, Plata, Oro, Platino, Diamante
        public double PuntosMinimos { get; set; }
        public double PuntosMaximos { get; set; }
    }
}
