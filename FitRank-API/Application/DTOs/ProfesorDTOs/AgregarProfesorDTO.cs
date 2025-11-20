namespace FitRank_API.Application.DTOs.ProfesorDTOs
{
    public class AgregarProfesorDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public int Dni { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Matricula { get; set; } = string.Empty;
        public double Sueldo { get; set; }

        public string Password { get; set; } = string.Empty;

        // Opcionales
        public long? GimnasioId { get; set; }
    }
}
