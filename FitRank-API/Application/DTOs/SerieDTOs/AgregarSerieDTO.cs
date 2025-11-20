namespace FitRank_API.Application.DTOs.SerieDTOs
{
    public class AgregarSerieDTO
    {
        public int NumeroDeSerie { get; set; }
        public TimeSpan? Duracion { get; set; } // Nullable TimeSpan
        public int? Repeticiones { get; set; }
        public double? Peso { get; set; }
        public long EjercicioAsignadoId { get; set; }
    }
}
