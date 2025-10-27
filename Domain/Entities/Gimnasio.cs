namespace FitRank_API.Domain.Entities
{
    public class Gimnasio
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string RazonSocial { get; set; } = string.Empty; 
        public string LogoUrl { get; set; } = string.Empty;
        public string ColorPrincipal { get; set; }
        public string ColorSecundario { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Cuil { get; set; } = string.Empty;

        public ICollection<Socio>? Socios { get; set; }
        public ICollection<Invitacion>? Invitaciones { get; set; }
        public ICollection<Asistencia>? Asistencias { get; set; }
    }
}
