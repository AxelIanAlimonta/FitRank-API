using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitRank_API.Domain.Entities
{
    [Table("SocioRealizaLogro")]
    [Index(nameof(SocioId), nameof(LogroId), IsUnique = true)]
    public class SocioRealizaLogro
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public int SocioId { get; set; }

        [Required]
        public int LogroId { get; set; }

        [Required]
        public int PuntosOtorgados { get; set; }

        [Required]
        public DateTime FechaOtorgado { get; set; } = DateTime.UtcNow;

        // FK
        [ForeignKey(nameof(SocioId))]
        public Socio? Socio { get; set; }

        [ForeignKey(nameof(LogroId))]
        public Logro? Logro { get; set; }

        [Required]
        public int GimnasioId { get; set; }
        [ForeignKey(nameof(GimnasioId))]
        public Gimnasio? Gimnasio { get; set; }
    }
}
