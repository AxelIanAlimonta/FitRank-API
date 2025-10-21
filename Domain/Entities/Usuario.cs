namespace FitRank_API.Domain.Entities
{
    public class Usuario
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public int Dni { get; set; }
        public string Correo { get; set; } = string.Empty;
        public int alturaCm { get; set; }
        public double pesoKg { get; set; }
        public string? nivel { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string? QrToken { get; set; } = string.Empty;
        public string? TokenRecuperacion { get; set; } = string.Empty;
        public DateTime? TokenExpira { get; set; }
        public bool EstaActivo { get; set; }
        public DateTime FechaNacimiento { get; set; }
        //public string Telefono { get; set; }
        public string? Estado { get; set; }
        public string Email { get; set; }
        public DateTime? CuotaPagadaHasta { get; set; }  // Ya nullable
        public string? telefono { get; set; }
        public bool EsActivado { get; set; } = false;

        //Coleccion de asistencias
        public ICollection<Asistencia> asistencias { get; set; } = new List<Asistencia>();

    }
}
