using System;

namespace FitRank_API.Application.DTOs.RutinaDTOs
{
    public class ObtenerRutinaDTO
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string TipoCreacion { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public string? Descripcion { get; set; } = string.Empty;
        public bool Activa { get; set; }
        public long SocioId { get; set; }
        public long UsuarioId { get; set; }
    }
}
