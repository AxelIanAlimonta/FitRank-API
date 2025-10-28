namespace FitRank_API.Domain.Entities
{
    public class MedidaCorporal
    {

        public long Id { get; set; }
        public long SocioId { get; set; }
        public Socio Socio { get; set; } = null!;
        public DateTime Fecha { get; set; }= DateTime.UtcNow;

        public double PechoCm { get; set; }
        public double CinturaCm { get; set; }
        public double CaderaCm { get; set; }
        public double PesoKg { get; set; }
        public double BrazoDerechoCm { get; set; }
        public double BrazoIzquierdoCm { get; set; }



    }
}
