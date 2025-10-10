using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitRank_API.Domain.Entities
{
    [Table("Logro")]
    public class Logro
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string Nombre { get; set; } = null!;

        [Required, MaxLength(1000)]
        public string Descripcion { get; set; } = null!;

        [Required]
        public int PuntosOtorgados { get; set; }

        [Required]
        public bool Activo { get; set; } = true;

        public ICollection<SocioRealizaLogro> Otorgamientos { get; set; } = new List<SocioRealizaLogro>();
    }
}
