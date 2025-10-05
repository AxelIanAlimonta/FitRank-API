namespace FitRank_API.Domain.Entities
{
    public class PuntuacionDiaria
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public DateTime Fecha { get; set; }

        public double PuntosAsistencia { get; set; }
        public double PuntosEjercicios { get; set; }
        public double Total => PuntosAsistencia + PuntosEjercicios;
    }
}
