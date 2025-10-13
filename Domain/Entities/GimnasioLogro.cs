using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitRank_API.Domain.Entities
{
    [Table("GimnasioLogro")]
    [Index(nameof(GimnasioId), nameof(LogroId), IsUnique = true)]
    public class GimnasioLogro
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int GimnasioId { get; set; }
        [Required]
        public int LogroId { get; set; }
        public bool Activo { get; set; } = true;
        [ForeignKey(nameof(GimnasioId))]
        public Gimnasio Gimnasio { get; set; } = null!;
        [ForeignKey(nameof(LogroId))]
        public Logro Logro { get; set; } = null!;
    }
}
