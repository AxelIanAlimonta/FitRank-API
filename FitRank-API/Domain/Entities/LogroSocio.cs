namespace FitRank_API.Domain.Entities
{
    public class LogroSocio
    {
        public long Id { get; set; }
        public long LogroId { get; set; }
        public long GimnasioId { get; set; }
        public long SocioId { get; set; }
        public DateTime FechaObtenido { get; set; }
        public Logro Logro { get; set; } = null!;
        public Gimnasio Gimnasio { get; set; } = null!;
        public Socio Socio { get; set; } = null!;
    }
}
