namespace FitRank_API.Application.DTOs.PuntajeDTOs
{
    public class ActualizarPuntajeDTO
    {
        public long Id { get; set; }
        public long SerieRealizadaId { get; set; }
        public string Motivo { get; set; }
        public DateTime Fecha { get; set; }
        public int Valor { get; set; }
    }
}
