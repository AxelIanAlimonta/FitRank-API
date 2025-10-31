namespace FitRank_API.Domain.Entities
{
    public class Entrenamiento
    {
        public long Id { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime Duracion { get; set; }

        public Socio Socio { get; set; } = null!;
        public long SocioId { get; set; }

        public ICollection<Actividad> Actividades { get; set; } = new List<Actividad>();
    }
}