namespace FitRank_API.Domain.Entities
{
    public class Jornada
    {
        public long Id { get; set; }
        public TimeSpan HoraInicio { get; set; } //Solo representa la hora del día: 08:00:00
        public TimeSpan HoraFin { get; set; } //Solo representa la hora del día: 14:00:00

        public long ProfesorId { get; set; }
        public Profesor? Profesor { get; set; }

        public long DiaDeLaSemanaId { get; set; }
        public DiaDeLaSemana? DiaDeLaSemana { get; set; }
    }
}
