namespace FitRank_API.Application.DTOs.AmistadDTOs
{
    public class EnviarSolicitudAmistadDTO
    {
        public int SolicitanteId { get; set; }   // se setea con el id del usuario logueado
        public int DestinatarioId { get; set; }
    }
}
