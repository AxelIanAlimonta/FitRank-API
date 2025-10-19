using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.DTOs.SerieRealizadaDTOs
{
    public class ObtenerSerieRealizadaDTO
    {
        public long Id { set; get; }
        public int Repeticiones { set; get; }
        public double Peso { set; get; }
        public int Rir { set; get; }
        public int NumeroDeSerie { set; get; }
        public long EjercicioRealizadoId { set; get; }
        public EjercicioRealizado? EjercicioRealizado { set; get; } // Propiedad de navegación

    }
}
