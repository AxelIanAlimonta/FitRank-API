namespace FitRank_API.Infrastructure.Persistence
{
    public class DiaEntity
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public List<BloqueDiaEntity> BloquesDias { get; set; } = new();
    }
}
