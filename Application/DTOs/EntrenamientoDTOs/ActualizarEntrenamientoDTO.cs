namespace FitRank_API.Application.DTOs.EntrenamientoDTOs
{
        public class ActualizarEntrenamientoDTO
        {
                public long Id { get; set; }
                public TimeSpan? Duracion { get; set; }
                public DateTime Fecha { get; set; }
                public long SocioId { get; set; }
        }
}
