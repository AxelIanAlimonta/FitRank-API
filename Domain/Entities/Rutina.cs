namespace FitRank_API.Domain.Entities
{
    public class Rutina
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int DiasPorSemana { get; set; }

        public ICollection<Ejercicio> Ejercicios { get; set; }
    }
}
