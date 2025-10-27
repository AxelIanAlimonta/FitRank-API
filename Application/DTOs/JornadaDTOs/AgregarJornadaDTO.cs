namespace FitRank_API.Application.DTOs.JornadaDTOs
{
    public class AgregarJornadaDTO
    {
        public long ProfesorId { get; set; }
        public long DiaDeLaSemanaId { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
    }
}
