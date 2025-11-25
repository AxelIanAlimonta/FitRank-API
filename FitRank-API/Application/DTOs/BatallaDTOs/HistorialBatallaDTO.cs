using FitRank_API.Domain.Enums;

namespace FitRank_API.Application.DTOs.BatallaDTOs
{
    public class HistorialBatallaDTO
    {
        public long BatallaId { get; set; }
        public string Oponente { get; set; }
        public BatallaEstado Estado { get; set; }
        public double PuntosA { get; set; }
        public double PuntosB { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool UsuarioEsA { get; set; }
        public int UsuarioA { get; set; }
        public int UsuarioB { get; set; }
    }
}
