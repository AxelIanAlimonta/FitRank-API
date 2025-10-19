namespace FitRank_API.Application.DTOs.SesionRealizadaDeEjercicios
{
    public class AgregarSesionRealizadaDeEjerciciosDTO
    {
        public DateTime Fecha { get; set; }
        public int DuracionEnMinutos { get; set; }
        public long NumeroDeSesion { get; set; }
    }
}
