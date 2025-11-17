namespace FitRank_API.Application.DTOs.ProfesorDTOs
{
    public class ProfesorDTO
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public int Dni { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string FotoDePerfil { get; set; } = string.Empty;
        public string? Estado { get; set; }
        public bool EsActivado { get; set; }

        // Campos propios de Profesor
        public string Matricula { get; set; } = string.Empty;
        public double Sueldo { get; set; }

        // Opcionales
        public long? GimnasioId { get; set; }
        public string? GimnasioNombre { get; set; }
    }
}
