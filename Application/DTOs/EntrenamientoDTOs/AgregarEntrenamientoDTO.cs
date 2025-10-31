namespace FitRank_API.Application.DTOs.EntrenamientoDTOs
{
    public class AgregarEntrenamientoDTO
    {
        public DateTime? Fecha { get; set; }
        public DateTime? Duracion { get; set; }
        public long SocioId { get; set; }
    }
}
