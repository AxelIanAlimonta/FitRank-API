namespace FitRank_API.Application.DTOs.AmistadDTOs
{
    public class SolicitudAmistadDTO
    {
        public long AmistadId { get; set; }
        public long RemitenteId { get; set; }
        public string RemitenteNombreUsuario { get; set; } = null!;
        public string RemitenteNombre { get; set; } = null!;
        public double RemitentePuntaje { get; set; }
    }
}
