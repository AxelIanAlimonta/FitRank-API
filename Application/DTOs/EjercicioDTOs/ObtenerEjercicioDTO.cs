namespace FitRank_API.Application.DTOs.EjercicioDTOs.ObtenerEjercicioDTO
{
    public class ObtenerEjercicioDTO
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; } = string.Empty;
        public string UrlImagen { get; set; } = string.Empty;
        public int DuracionEstimada { get; set; }
        public string UrlVideo { get; set; } = string.Empty;
        public long GrupoMuscularId { get; set; }
        public string? NombreGrupoMuscular { get; set; }
        public long? MaquinaId { get; set; }
        public string? NombreMaquina { get; set; }
    }
}
