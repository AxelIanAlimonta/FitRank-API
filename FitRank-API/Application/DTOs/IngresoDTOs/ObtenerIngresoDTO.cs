namespace FitRank_API.Application.DTOs.IngresoDTOs
{
    public class ObtenerIngresoDTO
    {
        public long Id { get; set; }
        public long GimnasioId { get; set; }
        public long? UsuarioId { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = "Efectivo";
        public DateTime Fecha { get; set; }
        public string? Observaciones { get; set; }
        public bool Confirmado { get; set; }
    }
}
