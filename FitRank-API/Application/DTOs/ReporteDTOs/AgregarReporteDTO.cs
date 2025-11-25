namespace FitRank_API.Application.DTOs.ReporteDTOs
{
    public class AgregarReporteDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public long UsuarioId { get; set; }
        public long GimnasioId { get; set; }
    }
}
