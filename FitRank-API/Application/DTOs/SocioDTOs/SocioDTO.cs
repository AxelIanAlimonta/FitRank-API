using System.Reflection.PortableExecutable;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.DTOs.SocioDTOs;



    public class SocioDTO
    {
        // 🔹 Heredadas de Usuario
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public int Dni { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;
        public DateTime? CuotaPagadaHasta { get; set; }
        public string? FotoUrl { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }

    public long? GimnasioId { get; set; }
        public string? GimnasioNombre { get; set; }
        public DateTime FechaRegistro { get; set; }
        public double Altura { get; set; }
        public double Peso { get; set; }
        public string Nivel { get; set; } = string.Empty;
        public double Puntaje { get; set; }
        public bool ParticipaEnRanking { get; set; }


        public string? QrToken { get; set; } = string.Empty;
    }


