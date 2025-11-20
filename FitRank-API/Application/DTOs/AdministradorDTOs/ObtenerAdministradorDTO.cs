namespace FitRank_API.Application.DTOs.AdministradorDTOs
{
    public class ObtenerAdministradorDTO
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Cuil { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Localidad { get; set; } = string.Empty;

        public long? GimnasioId
        {
            get; set;

        }
    }
}