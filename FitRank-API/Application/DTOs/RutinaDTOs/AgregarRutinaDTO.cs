namespace FitRank_API.Application.DTOs.RutinaDTOs
{
    public class AgregarRutinaDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string TipoCreacion { get; set; } = string.Empty;
        public string? Descripcion { get; set; } = string.Empty;
        public bool Activa { get; set; } = true;
        public long SocioId { get; set; }
        public long UsuarioId { get; set; }
    }
}
