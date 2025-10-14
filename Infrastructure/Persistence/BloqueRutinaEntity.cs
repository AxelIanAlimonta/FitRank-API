namespace FitRank_API.Infrastructure.Persistence
{
    public class BloqueRutinaEntity
    {
        public int Id { get; set; }
        public int IdRutina { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public List<BloqueDiaEntity> Dias { get; set; } = new();
        public List<EjercicioBloqueEntity> Ejercicios { get; set; } = new();

        public RutinaEntity Rutina { get; set; } // Evito la consulta a la bdd
    }
}
