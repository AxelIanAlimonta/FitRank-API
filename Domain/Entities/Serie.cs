namespace FitRank_API.Domain.Entities
{
    public class Serie
    {
        public long Id { get; set; }

        public int NumeroDeSerie { get; set; }
        public TimeSpan? Duracion { get; set; } 
        public int? Repeticiones { get; set; }
        public double? Peso { get; set; }

        public long EjercicioAsignadoId { get; set; }
        public EjercicioAsignado EjercicioAsignado { get; set; } = null!;

        public ICollection<Actividad> Actividades { get; set; }
    }
}
