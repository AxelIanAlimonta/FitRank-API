namespace FitRank_API.Application.DTOs.ActividadDTOs
{
    public class ObtenerActividadConPuntajeDTO
    {
        public long SerieId { get; set; }
        public string NombreEjercicio { get; set; } = string.Empty;
        public int Repeticiones { get; set; }
        public double Peso { get; set; }
        public double Puntos { get; set; }
        public bool PesoAjustado { get; set; }
        public string? MensajeAdvertencia { get; set; }
    }
}
