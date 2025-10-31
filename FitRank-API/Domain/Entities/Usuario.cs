namespace FitRank_API.Domain.Entities
{
    public class Usuario
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public int Dni { get; set; }
     
      
        public string NombreUsuario { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;

        public string Sexo { get; set; } = string.Empty;
        public string? QrToken { get; set; } = string.Empty;
        public string? TokenRecuperacion { get; set; } = string.Empty;
        public DateTime? TokenExpira { get; set; }

        public DateTime FechaNacimiento { get; set; }

        public string? FotoDePerfil { get; set; } = string.Empty;

        public string? Estado { get; set; }
        public string Email { get; set; }
        public DateTime? CuotaPagadaHasta { get; set; } 
        public string? Telefono { get; set; }
        public bool EsActivado { get; set; } = false;

        public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
        public ICollection<Notificacion> NotificacionesEnviadas { get; set; }
        public ICollection<Notificacion> NotificacionesRecibidas { get; set; }

        public ICollection<Rutina> RutinasCreadas { get; set; }

        public ICollection<Rutina> RutinasAsignadas { get; set; }

    }
}
