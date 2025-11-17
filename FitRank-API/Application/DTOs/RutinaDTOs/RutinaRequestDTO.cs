using System.Text.Json.Serialization;

namespace FitRank_API.Application.DTOs.RutinaDTOs
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum NivelEntrenamiento
    {
        Principiante,
        Intermedio,
        Avanzado
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ObjetivoPrincipal
    {
        Hipertrofia,
        PerdidaDePeso,
        Fuerza,
        Resistencia
    }

    public sealed class RutinaRequestDTO
    {
        public int Edad { get; set; }
        public double PesoKg { get; set; }
        public int AlturaCm { get; set; }
        public NivelEntrenamiento Nivel { get; set; }
        public int SesionesPorSemana { get; set; }
        public int MinutosPorSesion { get; set; }
        public ObjetivoPrincipal Objetivo { get; set; }
        public int CalidadAlimentacion { get; set; } //1 muy mala, 5 muy buena
        public int HorasSuenio { get; set; }
        public ScreeningDTO Screening { get; set; }
        public PreferenciasDTO Preferencias { get; set; }
    }
}
