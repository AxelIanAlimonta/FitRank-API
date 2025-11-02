namespace FitRank_API.Application.DTOs.SerieDTOs
{
    public class ActualizarSerieDTO
    {
        public long Id { get; set; }
        public int NumeroDeSerie { get; set; }
        public TimeSpan? Duracion { get; set; } // Nullable TimeSpan
        public int? Repeticiones { get; set; }
        public double? Peso { get; set; }
    }
}
