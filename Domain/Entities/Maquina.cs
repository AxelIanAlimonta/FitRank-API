namespace FitRank_API.Domain.Entities
{
    public class Maquina
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string CodigoQR { get; set; }
        public string VideoBaseUrl { get; set; }
        public ICollection<Ejercicio> Ejercicios { get; set; }
    }
}
