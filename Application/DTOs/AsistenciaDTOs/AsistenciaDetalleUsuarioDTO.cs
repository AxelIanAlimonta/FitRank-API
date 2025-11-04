namespace FitRank_API.Application.DTOs.Asistencia
{
    public class AsistenciaDetalleUsuarioDTO
    {
        public DateTime Fecha { get; set; }
        public DateTime HoraEntrada { get; set; }
        public DateTime? HoraSalida { get; set; }
        public string? Observaciones { get; set; }
        public string GimnasioNombre { get; set; } = string.Empty;
    }
}
