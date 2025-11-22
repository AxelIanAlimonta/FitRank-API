namespace FitRank_API.Application.DTOs.SocioDTOs
{
    public class EditarPerfilSocioDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public double Altura { get; set; }
        public double Peso { get; set; }
    }
}
