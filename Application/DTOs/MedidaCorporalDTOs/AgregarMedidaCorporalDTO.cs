namespace FitRank_API.Application.DTOs.MedidaCorporalDTOs
{
    public class AgregarMedidaCorporalDTO
    {
        public long SocioId { get; set; }

        public double PechoCm { get; set; }
        public double CinturaCm { get; set; }
        public double CaderaCm { get; set; }
        public double PesoKg { get; set; }
        public double BrazoDerechoCm { get; set; }
        public double BrazoIzquierdoCm { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}
