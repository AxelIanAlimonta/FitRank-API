namespace FitRank_API.Domain.Entities
{
    public class Dia
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<BloqueDia> BloquesDias { get; set; } = new(); //Que bloques estan asignados a ese día
    }
}
