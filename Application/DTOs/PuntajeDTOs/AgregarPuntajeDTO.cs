using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.DTOs.PuntajeDTOs
{
    public class AgregarPuntajeDTO
    {
        public long SerieRealizadaId { get; set; }
        public string Motivo { get; set; }
        public DateTime Fecha { get; set; }
        public int Valor { get; set; }
        public long SocioId { set; get; } //FK
    }
}
