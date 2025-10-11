namespace FitRank_API.Application.DTOs.Rutina
{
    public class EjercicioBloqueDTO
    {
        public int IdEjercicio { get; set; }
        public int Orden { get; set; }
        public int Series { get; set; }
        public int Repeticiones { get; set; }
        public int Rir { get; set; }
        public decimal? Peso { get; set; }
    }
}
