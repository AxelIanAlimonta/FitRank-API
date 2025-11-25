using FitRank_API.Domain.Enums;

namespace FitRank_API.Application.DTOs.BatallaDTOs
{
    public class ResultadoBatallaDTO
    {
        public long BatallaId { get; set; }
        public double PuntosJugadorA { get; set; }
        public double PuntosJugadorB { get; set; }
        public BatallaEstado Estado { get; set; }
        public long? GanadorId { get; set; }  // "A", "B" o "Empate"
    }
}
