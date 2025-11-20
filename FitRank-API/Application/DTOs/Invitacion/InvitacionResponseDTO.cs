namespace FitRank_API.Application.DTOs.Invitacion
{
    public class InvitacionResponseDTO
    {
        public bool Success { get; set; }
        public string QrImage { get; set; } = string.Empty;
        public string TokenInvitacion { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public long InvitacionId { get; set; }
        public string? LinkPago { get; set; } 

    }
}
