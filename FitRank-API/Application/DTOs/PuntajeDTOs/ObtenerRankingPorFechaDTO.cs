namespace FitRank_API.Application.DTOs.PuntajeDTOs
{
    public class ObtenerRankingPorFechaDTO
    {
            public long SocioId { get; set; }
            public string NombreCompleto { get; set; } = string.Empty;
            public double PuntajeTotal { get; set; }
            public DateOnly Desde { get; set; }
            public DateOnly Hasta { get; set; }

    }
}
