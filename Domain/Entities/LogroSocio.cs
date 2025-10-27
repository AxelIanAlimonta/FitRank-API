namespace FitRank_API.Domain.Entities
{
    public class LogroSocio
    {
        public long Id { get; set; }
        public long LogroId { get; set; }
        public Logro Logro { get; set; }
        public long SocioId { get; set; }
        public int PuntosOtorgados { get; set; }
        public DateTime FechaOtorgado { get; set; } = DateTime.UtcNow;
    }
}
