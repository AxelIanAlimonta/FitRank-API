namespace FitRank_API.Application.DTOs.Logro
{
    public class LogroCreateDto
    {
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public int Puntos { get; set; }
        public bool Activo { get; set; } = true;
    }
}
