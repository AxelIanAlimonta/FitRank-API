using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitRank_API.Domain.Entities
{
    public class Sesion
    {
        [Key]
        public long Id { get; set; }
        public int NumeroDeSesion { get; set; }
        public string Nombre { get; set; }

        [ForeignKey("Rutina")]
        public long RutinaId { get; set; }
        public Rutina Rutina { get;set; }

        public ICollection<EjercicioAsignado>? EjerciciosAsignados { get; set; }
    }
}
