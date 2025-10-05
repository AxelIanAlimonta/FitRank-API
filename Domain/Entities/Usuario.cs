namespace FitRank_API.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string telefono { get; set; }
        public string Estado { get; set; }
        public string correo { get; set; }
        public int AlturaCm { get; set; }
        public double PesoKg { get; set; }
        public string Nivel { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }
        public ICollection<Rutina> Rutinas { get; set; }
        public ICollection<Asistencia> Asistencias { get; set; }
        public ICollection<PuntuacionDiaria> PuntuacionesDiarias { get; set; }
        public ICollection<Ranking> Rankings { get; set; }

    }
}
