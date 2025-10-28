namespace FitRank_API.Application.DTOs.SerieDTOs
{
    public class AgregarSerieDTO
    {
        public int NumeroDeSerie { get; set; }
        public DateTime? Duracion { get; set; }
        public int? Repeticiones { get; set; }
        public double? Peso { get; set; }
        public long EjercicioAsignadoId { get; set; }
    }
}
