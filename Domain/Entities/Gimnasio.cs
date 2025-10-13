namespace FitRank_API.Domain.Entities
{
    public class Gimnasio
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Direccion { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string Email { get; set; } = null!;
        public ICollection<Socio> Socios { get; set; } = new List<Socio>();
        public ICollection<GimnasioLogro> Logros { get; set; } = new List<GimnasioLogro>();
    }
}