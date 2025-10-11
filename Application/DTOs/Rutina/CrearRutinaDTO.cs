namespace FitRank_API.Application.DTOs.Rutina
{
    public class CrearRutinaDTO
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public int FrecuenciaSemanal { get; set; }
        public List<BloqueRutinaDTO> Bloques { get; set; } = new();
    }
}
