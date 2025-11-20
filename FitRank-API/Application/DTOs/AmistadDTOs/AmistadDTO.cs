namespace FitRank_API.Application.DTOs.AmistadDTOs
{
    public class AmistadDTO
    {
        public bool Completado { get; set; }
        public string Mensaje { get; set; } = string.Empty;

        public long? AmistadId { get; set; }
        public long? SocioId1 { get; set; }
        public long? SocioId2 { get; set; }
        public long? SolicitanteId { get; set; }
        public string? Estado { get; set; }
    }
}
