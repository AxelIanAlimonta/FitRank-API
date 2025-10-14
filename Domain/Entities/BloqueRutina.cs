namespace FitRank_API.Domain.Entities
{
    public class BloqueRutina
    {
        public int Id { get; set; }
        public int IdRutina { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; } //Consultar si dejar este atributo
        public List<BloqueDia> Dias { get; set; } = new(); //Dias que se hace este bloque
        public List<EjercicioBloque> Ejercicios { get; set; } = new(); //Ejercicios que se realizan
    }
}
