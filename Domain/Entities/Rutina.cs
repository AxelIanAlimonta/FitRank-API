namespace FitRank_API.Domain.Entities
{
    public class Rutina
    {
        public int Id { get; set; }
        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int DiasPorSemana { get; set; }

        public ICollection<Ejercicio> Ejercicios { get; set; }
        public int FrecuenciaSemanal {  get; set; } // 1/2/3, cuantas veces se hace la rutina completa
        public List<BloqueRutina> bloques { get; set; } = new(); // fullBody, torso, etc.
    }
}
