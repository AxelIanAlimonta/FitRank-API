namespace FitRank_API.Application.DTOs.Auth.invitacion
{
    public class InvitacionResponseDto
    {
        public bool Success { get; set; }
        public string QrImage { get; set; } = string.Empty;
        public string TokenInvitacion { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public int InvitacionId { get; set; }
    }
}
