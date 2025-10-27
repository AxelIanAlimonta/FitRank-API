using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitRank_API.Domain.Entities
{
    public class Rutina
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "El nombre de la rutina es obligatorio")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string TipoCreacion { get; set; } = string.Empty;

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Descripcion { get; set; } = string.Empty;

        public bool Activa { get; set; } = true;

        // 🔗 Relaciones
        [ForeignKey("Socio")]
        public long SocioId { get; set; }
        public Socio? Socio { get; set; }

        [ForeignKey("Usuario")]
        public long UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }


        // Una rutina puede tener muchas sesiones y sesiones realizadas
        public ICollection<Sesion>? Sesiones { get; set; }
        //public ICollection<SesionRealizada>? SesionesRealizadas { get; set; }
    }
}
