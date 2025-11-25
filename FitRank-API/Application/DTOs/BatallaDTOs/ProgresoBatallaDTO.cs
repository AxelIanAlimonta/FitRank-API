using FitRank_API.Domain.Enums;

namespace FitRank_API.Application.DTOs.BatallaDTOs
{
    public class ProgresoBatallaDTO
    {
        public int BatallaId { get; set; }
        public double PuntosJugadorA { get; set; }
        public double PuntosJugadorB { get; set; }
        public double PuntosGuardadosA { get; set; }
        public double PuntosGuardadosB { get; set; }
        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }
        public BatallaEstado Estado { get; set; }
        public int UsuarioA { get; set; }
        public int UsuarioB { get; set; }
        public int? ganadorId { get; set; }
    }

}
