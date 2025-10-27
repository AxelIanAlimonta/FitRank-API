namespace FitRank_API.Domain.Entities
{
    public class Asistencia
    {
        public long Id { get; set; }
        public long UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public bool Presente { get; set; }
        public DateTime HoraEntrada { get; set; }
        public DateTime? HoraSalida { get; set; }
        public string Observaciones { get; set; }
     
        public long GimnasioId { get; set; }

        public Gimnasio Gimnasio { get; set; }
    }
}
