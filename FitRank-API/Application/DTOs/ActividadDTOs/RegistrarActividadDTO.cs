namespace FitRank_API.Application.DTOs.ActividadDTOs
{
    public class RegistrarActividadDTO
    {
        public long SerieId { get; set; }
        public int NumeroSerie { get; set; }
        public int Repeticiones { get; set; }
        public double Peso { get; set; }
        public TimeSpan? Duracion { get; set; }
    }
}
