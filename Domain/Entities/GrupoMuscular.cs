using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Domain.Entities
{
    public class GrupoMuscular
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "El nombre del grupo muscular es obligatorio")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Imagen { get; set; }

        // Relación con Ejercicio: un grupo muscular puede tener muchos ejercicios
        public ICollection<Ejercicio>? Ejercicios { get; set; }
    }
}
