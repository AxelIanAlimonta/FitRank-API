namespace FitRank_API.Application.DTOs.NotificacionDTOs
{
    public class EnviarIndividualDTO
    {
        public long UsuarioReceptorId { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
    }

}
