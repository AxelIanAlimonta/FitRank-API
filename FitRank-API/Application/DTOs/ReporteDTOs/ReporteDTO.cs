namespace FitRank_API.Application.DTOs.ReporteDTOs
{
    public class ReporteDTO
    {
        public long Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
        public long UsuarioId { get; set; }
        public long GimnasioId { get; set; }
    }
}
