namespace FitRank_API.Domain.Entities
{
    public class Pago
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }  // O InvitacionId si pre-registro
        public Usuario Usuario { get; set; } = null!;
        public decimal Monto { get; set; }  // En pesos
        public string MetodoPago { get; set; } = "Efectivo";  // 'Efectivo', 'MercadoPago'
        public string Periodo { get; set; } = "Monthly";  // 'Monthly', 'Yearly'
        public DateTime FechaPago { get; set; }
        public string? MpPaymentId { get; set; }  // ID externo para MP
        public string Estado { get; set; } = "Completado";  // 'Completado', 'Fallido', 'Pendiente'
        public string Observaciones { get; set; } = string.Empty;  // ej. "Fallback de MP"
    }
}
