namespace FitRank_API.Application.DTOs.ReporteDTOs
{
    public class ActualizarReporteDTO
    {
        public long Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
