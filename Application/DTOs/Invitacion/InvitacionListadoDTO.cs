namespace FitRank_API.Application.DTOs.Invitacion
{
    public class InvitacionListadoDTO
    {
        public long Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string MetodoPago { get; set; } = string.Empty;
        public DateTime CreadaEn { get; set; }
        public DateTime ExpiraEn { get; set; }
        public DateTime? CuotaPagadaHasta { get; set; }

    }
}
