namespace FitRank_API.Domain.Entities
{
    public class EjercicioBloque
    {
        public int Id { get; set; }
        public int IdBloqueRutina { get; set; }
        public int IdEjercicio { get; set; }
        public int Orden {  get; set; }
        public int Series { get; set; }
        public int Repeticiones { get; set; }
        public int Rir {  get; set; }
        public decimal Peso { get; set; }

        public BloqueRutina BloqueRutina { get; set; } //Permite obtener informacion sin hacer una consulta
        public Ejercicio Ejercicio { get; set; } //Permite obtener informacion sin hacer una consulta

    }
}
