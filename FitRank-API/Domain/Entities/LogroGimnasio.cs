namespace FitRank_API.Domain.Entities
{
    public class LogroGimnasio
    {
        public long Id { get; set; }
        public long GimnasioId { get; set; }
        public long LogroId { get; set; }
        public bool EstaActivo { get; set; }

        public Gimnasio Gimnasio { get; set; }
        public Logro Logro { get; set; }
    }

}
