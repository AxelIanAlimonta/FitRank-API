namespace FitRank_API.Application.DTOs.EntrenamientoDTOs
{
    public class ObtenerEntrenamientoDTO
    {
        public long Id { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan? Duracion { get; set; }
        public long SocioId { get; set; }
    }
}