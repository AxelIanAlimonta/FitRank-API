namespace FitRank_API.Application.DTOs.ActividadDTOs
{
    public class ActualizarActividadDTO
    {
        public long Id { get; set; }
        public DateTime? Duracion { get; set; }
        public int? Repeticiones { get; set; }
        public double? Peso { get; set; }
        public double? Punto { get; set; }
    }
}
