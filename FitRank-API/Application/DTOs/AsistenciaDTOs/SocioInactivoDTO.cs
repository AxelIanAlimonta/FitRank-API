namespace FitRank_API.Application.DTOs.Asistencia
{
    public class SocioInactivoDTO
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long GimnasioId { get; set; }
        public int DiasSinAsistir { get; set; }

        public string? Telefono { get; set; }
    }
}
