namespace FitRank_API.Application.DTOs.EjercicioDTOs.AgregarEjercicioDTO
{
    public class AgregarEjercicioDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; } = string.Empty;
        public string UrlImagen { get; set; } = string.Empty;
        public int DuracionEstimada { get; set; }
        public string UrlVideo { get; set; } = string.Empty;
        public long GrupoMuscularId { get; set; }
        public long? MaquinaId { get; set; }
        public List<string> ContraIndicaciones { get; set; } = new List<string>();
    }
}
