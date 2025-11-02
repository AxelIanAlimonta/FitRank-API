namespace FitRank_API.Domain.Entities
{
    public class Actividad
    {
        public long Id { get; set; }
        public long SerieId { get; set; }
        public Serie Serie { get; set; }

        public TimeSpan? Duracion { get; set; }
        public int? Repeticiones { get; set; }
        public double? Peso { get; set; }
        public double? Punto { get; set; }

        public long EntrenamientoId { get; set; }
        public Entrenamiento Entrenamiento { get; set; }

        public long EjercicioAsignadoId { get; set; }
        public EjercicioAsignado EjercicioAsignado { get; set; }
    }
}