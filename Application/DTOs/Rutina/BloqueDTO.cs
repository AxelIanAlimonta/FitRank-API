namespace FitRank_API.Application.DTOs.RutinaNamespace
{
    public class BloqueDTO
    {
        public int Id { get; set; }
        public int RutinaId { get; set; }
        public int EjercicioId { get; set; }

        public DayOfWeek Dia { get; set; }
        public int SeriesRecomendadas { get; set; }
        public int RepeticionesRecomendadas { get; set; }
        public double PesoRecomendado { get; set; }
        public int RirRecomendado { get; set; }
        public string? Notas { get; set; }
    }
}
