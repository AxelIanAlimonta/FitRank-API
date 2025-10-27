namespace FitRank_API.Application.DTOs.AdministradorDTOs
{
    public class AgregarAdministradorDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public int Dni { get; set; }
        public string Email { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Cuil { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Localidad { get; set; } = string.Empty;
    }
}
