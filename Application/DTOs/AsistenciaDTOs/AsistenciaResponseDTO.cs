namespace FitRank_API.Application.DTOs.Asistencia
{
    public class AsistenciaResponseDTO
    {
        public bool Success { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public long AsistenciaId { get; set; }
        public string? NombreUsuario { get; set; }
        public DateTime? HoraEntrada { get; set; }
    }
}
