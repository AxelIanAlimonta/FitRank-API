namespace FitRank_API.Application.DTOs.NotificacionDTOs
{
    public class ObtenerNotificacionDTO
    {
        public long Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public bool Leido { get; set; }
        public bool Activa { get; set; }
        public DateTime FechaEnvio { get; set; }
        public long UsuarioEmisorId { get; set; }
        public long UsuarioReceptorId { get; set; }
    }
}
