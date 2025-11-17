namespace FitRank_API.Application.DTOs.EntrenamientoDTOs
{
        public class AgregarEntrenamientoDTO
        {
                public DateTime Fecha { get; set; } = DateTime.UtcNow;
                public TimeSpan? Duracion { get; set; }
                public long SocioId { get; set; }
        }
}
