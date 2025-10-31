namespace FitRank_API.Application.DTOs.MaquinaDTOs
{
    public class ObtenerMaquinaDTO
    {
        public long Id { get; set; }
        public long GimnasioId { get; set; }
        public string Nombre { get; set; }
        public string UrlImagen { get; set; }
        public string Qr { get; set; }
    }
}
