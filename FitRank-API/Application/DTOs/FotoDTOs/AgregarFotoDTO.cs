namespace FitRank_API.Application.DTOs.FotoDTOs
{
    public class AgregarFotoDTO
    {
        public long SocioId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public string UrlImagen { get; set; } = string.Empty;
    }
}
