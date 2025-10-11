namespace FitRank_API.Application.DTOs.Rutina
{
    public class RutinaDTO
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public int FrecuenciaSemanal { get; set; }
        public List<BloqueRutinaDTO> Bloques { get; set; } = new();
    }
}