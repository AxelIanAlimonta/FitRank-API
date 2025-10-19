using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.DTOs.PuntajeDTOs
{
    public class ObtenerPuntajeDTO
    {
        public long Id { set; get; }
        public long SerieRealizadaId { set; get; } //FK
        public SerieRealizada? SerieRealizada { set; get; } // Propiedad de navegación
        public string Motivo { set; get; }
        public DateTime Fecha { set; get; }
        public int Valor { set; get; }
    }
}
