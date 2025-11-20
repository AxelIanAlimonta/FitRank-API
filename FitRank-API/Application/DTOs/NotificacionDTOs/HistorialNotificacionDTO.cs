namespace FitRank_API.Application.DTOs.NotificacionDTOs
{
    public class HistorialNotificacionDTO
    {
        public long Id { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Emisor { get; set; }
        public string Receptor { get; set; }
    }

}
