namespace FitRank_API.Application.DTOs.CalcularPuntajeDTOs
{
    public class EstadisticaCorporalSocioDTO
    {
        public double Imc { get; set; }
        public string ClasificacionImc { get; set; } = string.Empty;
        public double Peso { get; set; }
        public double Altura { get; set; }
        public DateTime FechaMedicion { get; set; }
    }
}
