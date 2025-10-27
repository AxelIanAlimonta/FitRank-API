namespace FitRank_API.Domain.Entities
{
    public class Maquina
    {
        public long Id { get; set; }
        public long GimnasioId { get; set; }
        public Gimnasio Gimnasio { get; set; }

        public string Nombre { get; set; }
        public string UrlImagen { get; set; }
        public string Qr { get; set; }

    }
}
