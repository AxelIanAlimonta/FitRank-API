using FitRank_API.Application.DTOs.EjercicioDTOs;

namespace FitRank_API.Application.DTOs.SesionDTOs
{
    public class SesionPlanDTO
    {
        public string Nombre { get; set; } = string.Empty;

        // Orden o índice de la sesión dentro de la rutina
        public int NumeroDeSesion { get; set; }

        // Lista de ejercicios que forman parte de esta sesión
        public List<EjercicioPlanDTO> Ejercicios { get; set; } = new();
    }
}
