namespace FitRank_API.Application.DTOs.RutinaDTOs
{
    public class ResultadoGenerarRutinaDTO
    {
        public bool RequiereDerivacion { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public object? Decisiones { get; set; }
        public RutinaGeneradaPorIADTO? Rutina { get; set; }
    }
}
