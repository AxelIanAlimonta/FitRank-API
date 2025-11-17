namespace FitRank_API.Application.DTOs.RutinaDTOs
{
    public class RutinaProfesorDTO
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Activa { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Datos del socio asignado, si corresponde
        public string? SocioNombre { get; set; }

        // Por si querés mostrar el grupo muscular o tipo
        public string? Tipo { get; set; }
    }
}
