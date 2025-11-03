namespace FitRank_API.Application.DTOs.NotificacionDTOs;

public class AgregarNotificacionDTO
{
    public long Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public bool Activa { get; set; } = true;
    public bool Leido { get; set; } = false;
    public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;
    public long UsuarioEmisorId { get; set; }
    public long UsuarioReceptorId { get; set; }
}
