namespace FitRank_API.Domain.Entities
{
    public class Administrador : Usuario
    {
        public string Cuil { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Localidad { get; set; } = string.Empty;

      
        public Gimnasio? Gimnasio { get; set; } = null!;
        
    }
}