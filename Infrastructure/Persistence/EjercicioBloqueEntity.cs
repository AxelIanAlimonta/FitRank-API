namespace FitRank_API.Infrastructure.Persistence
{
    public class EjercicioBloqueEntity
    {
        public int Id { get; set; }
        public int IdBloqueRutina { get; set; }
        public int IdEjercicio { get; set; }
        public int Orden { get; set; }
        public int Series { get; set; }
        public int Repeticiones { get; set; }
        public int Rir { get; set; }
        public decimal? Peso { get; set; }

        public BloqueRutinaEntity BloqueRutina { get; set; } // Evito la consulta
        public EjercicioEntity Ejercicio { get; set; } // Evito la consulta
    }
}
