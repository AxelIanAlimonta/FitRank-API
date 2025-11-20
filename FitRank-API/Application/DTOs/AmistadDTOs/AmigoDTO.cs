namespace FitRank_API.Application.DTOs.AmistadDTOs
{
    public class AmigoDTO
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public double Puntaje { get; set; }
    }
}
