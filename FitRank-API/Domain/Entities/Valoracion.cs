namespace FitRank_API.Domain.Entities
{
    public class Valoracion
    {
        public long Id { get; set; }


        public long EmisorId { get; set; }
        public Usuario Emisor { get; set; } = null!;


        public long ReceptorId { get; set; }
        public Usuario Receptor { get; set; } = null!;

  
        public long? RutinaId { get; set; }
        public Rutina? Rutina { get; set; }

        public int Puntaje { get; set; } 
        public string? Comentario { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}
