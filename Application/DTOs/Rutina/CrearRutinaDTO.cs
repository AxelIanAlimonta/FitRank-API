using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Application.DTOs.Rutina
{
    public class CrearRutinaDTO
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public int FrecuenciaSemanal { get; set; }
        public List<CrearBloqueRutinaDTO> Bloques { get; set; } = new();
    }

    public class CrearBloqueRutinaDTO
    {
        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        // IDs de los días de la semana o del calendario
        public List<int> DiasIds { get; set; } = new();

        [Required]
        public List<CrearEjercicioBloqueDTO> Ejercicios { get; set; } = new();
    }

    public class CrearEjercicioBloqueDTO
    {
        [Required]
        public int IdEjercicio { get; set; }  // solo el ID del ejercicio existente

        [Required]
        public int Orden { get; set; }

        [Required]
        public int Series { get; set; }

        [Required]
        public int Repeticiones { get; set; }

        [Required]
        public int Rir { get; set; }

        public decimal? Peso { get; set; }
    }
}
