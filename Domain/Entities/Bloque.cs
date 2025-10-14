namespace FitRank_API.Domain.Entities
{
    public class Bloque
    {
        public int Id { get; set; }
        //RELACIONES
        public int RutinaId { get; set; }
        public Rutina Rutina { get; set; }
        public int EjercicioId { get; set; }
        public Ejercicio Ejercicio { get; set; }
        //PROPIOS
        public DayOfWeek Dia { get; set; }
        public int SeriesRecomendadas { get; set; }
        public int RepeticionesRecomendadas { get; set;}
        public double PesoRecomendado { get; set; }
        public int RirRecomendado { get; set;}
        public string? Notas { get; set; }
    }
}
