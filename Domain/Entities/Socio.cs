using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitRank_API.Domain.Entities
{
    [Table("Socio")]
    public class Socio
    {

        [Key]
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string Nombre { get; set; } = null!;

        [Required, MaxLength(120)]
        public string Apellido { get; set; } = null!;

        [Required, MaxLength(150)]
        public string Email { get; set; } = null!;

        [MaxLength(20)]
        public string? Telefono { get; set; }

        public DateTime FechaNacimiento { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public bool Activo { get; set; } = true;

        public ICollection<SocioRealizaLogro> LogrosOtorgados { get; set; } = new List<SocioRealizaLogro>();
    }
}
