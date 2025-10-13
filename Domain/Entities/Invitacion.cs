using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Domain.Entities
{
    public class Invitacion
    {
        public int Id { get; set; }
        [Required]
        public int GymId { get; set; }  // ID del admin/gym (ej. Usuario.id del admin logueado)
        [Required]
        public string Email { get; set; } = string.Empty;
        public string DatosPrellenados { get; set; } = string.Empty;  // JSON serializado: {nombre, apellidos, dni, telefono}
        public string MetodoPago { get; set; } = "Efectivo";  // 'Efectivo', 'MercadoPago', 'FallbackEfectivo'
        public string? MpPaymentId { get; set; }  // ID del pago en Mercado Pago (si aplica)
        public DateTime CreadaEn { get; set; }
        public DateTime ExpiraEn { get; set; }  // Expira en 24h para invitación
        public string Estado { get; set; } = "Pendiente";  // 'Pendiente', 'Pagado', 'Usada', 'Expirada', 'FallbackEfectivo'
        public int? UsuarioId { get; set; }  // FK a Usuario post-registro (opcional)
        public DateTime? CuotaPagadaHasta { get; set; }  // Fecha vencimiento cuota (set post-pago confirmado)
    }
}
