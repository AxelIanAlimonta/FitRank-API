namespace FitRank_API.Application.DTOs.Auth
{
    public class UsuarioAuthDTO
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Rol { get; set; } = "User";
        public DateTime? CuotaPagadaHasta { get; set; }
        public bool TieneCuotaPagada { get; set; }
        public string? QrToken { get; set; }
    }
}
