namespace FitRank_API.Application.DTOs.UsuarioDTOs
{
    public class UsuarioAuthDTO
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Rol { get; set; } = "User";
        public DateTime? CuotaPagadaHasta { get; set; }
        public bool TieneCuotaPagada { get; set; }
        public string? QrToken { get; set; }

        public long? GimnasioId { get; set; }
    }
}
