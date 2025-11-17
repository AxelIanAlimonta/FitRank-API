namespace FitRank_API.Domain.Entities
{
    public class Gimnasio
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Direccion { get; set; } = string.Empty;
        public string? RazonSocial { get; set; } = string.Empty;
        public string? LogoUrl { get; set; } = string.Empty;
        public string? ColorPrincipal { get; set; } = string.Empty;
        public string? ColorSecundario { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Telefono { get; set; } = string.Empty;
        public string? Cuil { get; set; } = string.Empty;
        public long? AdministradorId { get; set; }
        public Administrador? Administrador { get; set; } = null!;
        public ICollection<Socio>? Socios { get; set; }
        public ICollection<Invitacion>? Invitaciones { get; set; }
        public ICollection<Asistencia>? Asistencias { get; set; }
        public ICollection<Profesor>? Profesores { get; set; } = new List<Profesor>();

    }
}
