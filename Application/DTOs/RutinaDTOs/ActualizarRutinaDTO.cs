namespace FitRank_API.Application.DTOs.RutinaDTOs
{
    public class ActualizarRutinaDTO
    {
        public long Id { get; set; }
        public string? Nombre { get; set; }
        public string? TipoCreacion { get; set; }
        public string? Descripcion { get; set; }
        public bool? Activa { get; set; }
        public long? SocioId { get; set; }
    }
}
