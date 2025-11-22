namespace FitRank_API.Application.DTOs.SocioDTOs
{
    public class SocioConMedidasDTO
    {
        public SocioDTO Socio { get; set; } = null!;

        public MedidaCorporalDTO? UltimaMedida { get; set; }
    }

    public class MedidaCorporalDTO
    {
        public long Id { get; set; }
        public DateTime Fecha { get; set; }
        public double PechoCm { get; set; }
        public double CinturaCm { get; set; }
        public double CaderaCm { get; set; }
        public double BrazoDerechoCm { get; set; }
        public double BrazoIzquierdoCm { get; set; }
        public double PesoKg { get; set; }
    }
}
