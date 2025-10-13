using System.ComponentModel.DataAnnotations;
using FitRank_API.Application.DTOs.EjercicioRealizado;

namespace FitRank_API.Domain.Entities
{
    public class Usuario
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string apellidos { get; set; }
        public int dni { get; set; }
        public DateTime? fechaNacimiento { get; set; }
        public string? telefono { get; set; }
        public string? estado { get; set; }
        public string correo { get; set; }
        public int? alturaCm { get; set; } 
        public double? pesoKg { get; set; }

        public string? nivel { get; set; }

        public string? username { get; set; }

        public string email { get; set; }

        // NUEVOS CAMPOS (nullable inicialmente para no romper datos existentes)
        public string? PasswordHash { get; set; }  // Nullable; agrega [Required] después de actualizar usuarios
        public string? Rol { get; set; } = "User";  // Nullable; default en código si null
        public DateTime? CuotaPagadaHasta { get; set; }  // Ya nullable
        public string? QrToken { get; set; }  // Nullable

        public string? TokenRecuperacion { get; set; }
        public DateTime? TokenExpira { get; set; }

        public bool EsActivado { get; set; } = false;
        public ICollection<Rutina> rutinas { get; set; }
        public ICollection<Asistencia> asistencias { get; set; }
        public ICollection<PuntuacionDiaria> puntuacionesDiarias { get; set; }

        public ICollection<EjercicioRealizado> ejerciciosRealizados { get; set; }


    }
}
