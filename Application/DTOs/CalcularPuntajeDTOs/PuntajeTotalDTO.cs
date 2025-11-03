namespace FitRank_API.Application.DTOs.CalcularPuntajeDTOs
{
    public class PuntajeTotalDTO
    {
        public long SocioId { get; set; }
        public double PuntajeTotal { get; set; }
        public List<PuntajePorGrupoDTO> PuntajePorGrupo { get; set; } = new();
    }
}
