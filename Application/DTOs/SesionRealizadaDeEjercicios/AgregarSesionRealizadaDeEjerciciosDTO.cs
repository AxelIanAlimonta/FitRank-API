namespace FitRank_API.Application.DTOs.SesionRealizadaDeEjercicios
{
    public class AgregarSesionRealizadaDeEjerciciosDTO
    {
        public DateTime Fecha { get; set; }
        public TimeSpan Duracion { get; set; }
        public long NumeroDeSesion { get; set; }
    }
}
