namespace FitRank_API.Application.DTOs.MaquinaDTOs
{
    public class AgregarMaquinaDTO
    {
        public long GimnasioId { get; set; }
        public string Nombre { get; set; }
        public string UrlImagen { get; set; }
        public string Qr { get; set; }
    }
}
