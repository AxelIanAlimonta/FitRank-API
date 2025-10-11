namespace FitRank_API.Application.DTOs.Rutina
{
    public class BloqueRutinaDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public List<BloqueDiaDTO> Dias { get; set; } = new();
        public List<EjercicioBloqueDTO> Ejercicios { get; set; } = new();
    }
}
