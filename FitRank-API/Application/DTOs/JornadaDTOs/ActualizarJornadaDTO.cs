namespace FitRank_API.Application.DTOs.JornadaDTOs
{
    public class ActualizarJornadaDTO
    {
        public long Id { get; set; }
        public long ProfesorId { get; set; }
        public long DiaDeLaSemanaId { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

    }
}
