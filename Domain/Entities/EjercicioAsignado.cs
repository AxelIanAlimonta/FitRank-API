using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitRank_API.Domain.Entities
{
    public class EjercicioAsignado
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public int NumeroEjercicio { get; set; } // orden del ejercicio en la sesión

        // Relaciones
        [Required]
        public long EjercicioId { get; set; }
        [ForeignKey("EjercicioId")]
        public Ejercicio Ejercicio { get; set; } = null!;

        [Required]
        public long SesionId { get; set; }
        [ForeignKey("SesionId")]
        public Sesion Sesion { get; set; } = null!;

        // Serie (por ahora comentada)
        // public long? SerieId { get; set; }
        // public Seriea? Serie { get; set; }
    }
}