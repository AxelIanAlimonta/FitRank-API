using FitRank_API.Application.DTOs.EjercicioNamespace;

namespace FitRank_API.Application.DTOs.RutinaNamespace
{
    public class RutinaDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int DiasPorSemana { get; set; }

        public ICollection<BloqueDTO> Bloques { get; set; }

        public ICollection<EjercicioDTO> Ejercicios { get; set; }
    }
}
