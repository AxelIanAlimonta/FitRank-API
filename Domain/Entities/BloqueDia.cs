namespace FitRank_API.Domain.Entities
{
    public class BloqueDia
    {
        public int Id { get; set; }
        public int IdBloqueRutina { get; set; }
        public int IdDia { get; set; }
        public BloqueRutina BloqueRutina { get; set; } //Permite obtener informacion sin hacer una consulta
        public Dia Dia { get; set; } //Permite obtener informacion sin hacer una consulta
    }
}
