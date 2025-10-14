namespace FitRank_API.Infrastructure.Persistence
{
    public class EjercicioEntity
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public List<EjercicioBloqueEntity> EjerciciosBloques { get; set; } = new();
    }
}
