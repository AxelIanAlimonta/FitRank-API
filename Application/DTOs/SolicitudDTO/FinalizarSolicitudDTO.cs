namespace FitRank_API.Application.DTOs.SolicitudDTO
{
    public class FinalizarSolicitudDTO
    {
        public long SolicitudId { get; set; }
        public long RutinaId { get; set; }
        public string? MensajeProfesor { get; set; }
    }

}
