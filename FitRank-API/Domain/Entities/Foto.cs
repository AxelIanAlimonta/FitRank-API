namespace FitRank_API.Domain.Entities
{
    public class Foto
    {
        public long Id { get; set; }


        public long SocioId { get; set; }
        public Socio Socio { get; set; } = null!;

    
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public string UrlImagen { get; set; } = string.Empty;
    }
}
