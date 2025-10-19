namespace FitRank_API.Application.DTOs.SesionRealizadaDeEjercicios
{
    public class SesionRealizadaDeEjerciciosDTO
    {
        public long Id { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Duracion { get; set; }
        public int NumeroDeSesion { get; set; }
    }
}
