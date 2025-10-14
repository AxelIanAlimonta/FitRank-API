namespace FitRank_API.Infrastructure.Persistence
{
    public class RutinaEntity
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public int FrecuenciaSemanal { get; set; }

        public List<BloqueRutinaEntity> Bloques { get; set; } = new();
    }
}
