namespace FitRank_API.Domain.Entities
{
    public class Reporte
    {
        public long Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; } = true;
        public long UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public long GimnasioId { get; set; }
        public Gimnasio Gimnasio { get; set; } = null!;
    }
}
