namespace FitRank_API.Application.DTOs.RankingDTOs
{
    public class RankingDTO
    {
        public long SocioId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public int PuntajeTotal { get; set; }
    }
}
