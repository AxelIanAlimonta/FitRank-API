namespace FitRank_API.Application.DTOs.MaquinaDTOs
{
    public class ActualizarMaquinaDTO
    {
        public int Id { get; set; }
        public int GimnasioId { get; set; }
        public string? Nombre { get; set; }
        public string? UrlImagen { get; set; }
        public string? Qr { get; set; }
    }
}
