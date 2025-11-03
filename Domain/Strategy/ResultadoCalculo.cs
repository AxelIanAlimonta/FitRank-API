using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Strategy
{
    public class ResultadoCalculo
    {
        public double Puntos { get; set; }
        public double PesoUsado { get; set; }
        public double PesoMaximoPermitido { get; set; }
        public bool PesoAjustado { get; set; }
        public string? MensajeAdvertencia { get; set; }
    }
}