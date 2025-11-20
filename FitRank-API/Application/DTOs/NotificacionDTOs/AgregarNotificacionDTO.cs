namespace FitRank_API.Application.DTOs.NotificacionDTOs;

public class AgregarNotificacionDTO
{
    
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
 
   
    public long UsuarioEmisorId { get; set; }
    public long UsuarioReceptorId { get; set; }
}
