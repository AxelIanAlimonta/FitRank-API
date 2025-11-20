namespace FitRank_API.Application.DTOs.Asistencia
{
    public class AgregarAsistenciaDTO
    {
        public long UsuarioId { get; set; }
        public long GimnasioId { get; set; }
        public string? Observaciones { get; set; }
    }
}
