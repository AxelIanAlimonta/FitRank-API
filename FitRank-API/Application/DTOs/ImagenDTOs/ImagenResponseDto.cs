namespace FitRank_API.Application.DTOs.ImagenDTOs
{
    public class ImagenResponseDto
    {
        public string Key { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string NombreArchivo { get; set; } = string.Empty;
        public long TamanoBytes { get; set; }
        public DateTime? UltimaModificacion { get; set; }
        public string? ETag { get; set; }
    }
}
