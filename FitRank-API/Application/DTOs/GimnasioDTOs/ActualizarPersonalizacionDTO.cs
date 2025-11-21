namespace FitRank_API.Application.DTOs.GimnasioDTOs
{
    public class ActualizarPersonalizacionDTO
    {
        public long Id { get; set; }
        public string ColorPrincipal { get; set; } = string.Empty;
        public string ColorSecundario { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
    }
}
