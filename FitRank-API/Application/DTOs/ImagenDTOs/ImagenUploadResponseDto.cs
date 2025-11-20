namespace FitRank_API.Application.DTOs.ImagenDTOs
{
    public class ImagenUploadResponseDto
    {
        public string Key { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string NombreArchivo { get; set; } = string.Empty;
        public long TamanoBytes { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime FechaSubida { get; set; }
    }
}
