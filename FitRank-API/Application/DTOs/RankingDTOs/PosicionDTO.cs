namespace FitRank_API.Application.DTOs.RankingDTOs
{
    public class PosicionDTO
    {
        public long SocioId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public int PuntajeTotal { get; set; }
        public int Posicion { get; set; }
    }
}
