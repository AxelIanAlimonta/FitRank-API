using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Domain.Entities
{
    public class Ingreso
    {
        public long Id { get; set; }

        [Required]
        public long GimnasioId { get; set; }
        public Gimnasio Gimnasio { get; set; }

        [Required]
        public long? UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [Required]
        public decimal Monto { get; set; }

        [Required]
        public string MetodoPago { get; set; } = "Efectivo"; // o "MercadoPago"

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public string? Observaciones { get; set; }

        public bool Confirmado { get; set; } = true; // si el admin lo marca manualmente o webhook
    }
}
