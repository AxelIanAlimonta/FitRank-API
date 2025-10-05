namespace FitRank_API.Domain.Entities
{
    public class Asistencia
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public bool Presente { get; set; } 
        public DateTime HoraEntrada { get; set; }
        public DateTime? HoraSalida { get; set; }
        public string Observaciones { get; set; }

    }
}
