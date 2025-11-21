namespace FitRank_API.Application.DTOs.MaquinaDTOs
{
    public class EjercicioDeMaquinaDTO
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int? DuracionEstimada { get; set; }
        public string? UrlVideo { get; set; }
        public string? UrlImagen { get; set; }
        public string? GrupoMuscular { get; set; }
    }
}
