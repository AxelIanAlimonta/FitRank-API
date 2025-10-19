namespace FitRank_API.Domain.Entities
{
    public class SesionRealizadaDeEjercicios
    {
        public long Id { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Duracion { get; set; } // 00:45:00
        public int NumeroDeSesion { get; set; }
    }
}
