namespace FitRank_API.Application.DTOs.Rutina
{
    public class ActualizarRutinaDTO
    {
        public string nombre {  get; set; }
        public int FrecuenciaSemanal {  get; set; }
        public List<BloqueRutinaDTO> Bloques { get; set; } = new();
    }
}
