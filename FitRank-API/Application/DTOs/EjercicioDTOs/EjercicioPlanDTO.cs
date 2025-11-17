using FitRank_API.Application.DTOs.SerieDTOs;

namespace FitRank_API.Application.DTOs.EjercicioDTOs
{
    public class EjercicioPlanDTO
    {
        public long EjercicioId { get; set; }

        // Orden del ejercicio dentro de la sesión
        public int NumeroEjercicio { get; set; }

        // Cada ejercicio tiene una lista de series
        public List<SeriePlanDTO> Series { get; set; } = new();
    }
}
