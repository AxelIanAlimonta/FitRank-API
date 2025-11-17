namespace FitRank_API.Application.DTOs.GimnasioDTOs
{
    public class ActualizarGimnasioDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Direccion { get; set; }
        public string? RazonSocial { get; set; }
        public string? LogoUrl { get; set; }
        public string? ColorPrincipal { get; set; }
        public string? ColorSecundario { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Cuil { get; set; }
        public long? AdministradorId { get; set; }
    }
}
