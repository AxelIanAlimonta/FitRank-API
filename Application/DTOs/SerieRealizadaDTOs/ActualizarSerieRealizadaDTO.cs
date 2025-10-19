namespace FitRank_API.Application.DTOs.SerieRealizadaDTOs
{
    public class ActualizarSerieRealizadaDTO
    {
        public long Id { set; get; }
        public int Repeticiones { set; get; }
        public double Peso { set; get; }
        public int Rir { set; get; }
        public int NumeroDeSerie { set; get; }
        public long EjercicioRealizadoId { set; get; }
    }
}
