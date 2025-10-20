namespace FitRank_API.Domain.Entities
{
    public class Asistencia
    {
        public int Id { get; set; }
        public long UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public bool Presente { get; set; }
        public DateTime HoraEntrada { get; set; }
        public DateTime? HoraSalida { get; set; }
        public string Observaciones { get; set; }
        // NUEVO: GymId = ID del admin que valida el acceso (trackea "quién escaneó")
        public int GymId { get; set; }  // Set con ID admin logueado en controller
    }
}
