namespace FitRank_API.Application.DTOs.IngresoDTOs
{
    public class AgregarIngresoDTO
    {
        public long GimnasioId { get; set; }
        public long? UsuarioId { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = "Efectivo";
        public string? Observaciones { get; set; }
    }
}
