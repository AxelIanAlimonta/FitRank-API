using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitRank_API.Domain.Entities
{
    public class EjercicioAsignado
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public int NumeroEjercicio { get; set; } 

       
        [Required]
        public long EjercicioId { get; set; }
        [ForeignKey("EjercicioId")]
        public Ejercicio Ejercicio { get; set; } = null!;

        [Required]
        public long SesionId { get; set; }
        [ForeignKey("SesionId")]
        public Sesion Sesion { get; set; } = null!;

        public ICollection<Serie>? Series { get; set; }

    }
}