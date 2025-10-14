using FitRank_API.Application.DTOs.Ejercicionamespace;

namespace FitRank_API.Application.DTOs.RutinaNamespace
{
    public class CrearRutinaDTO
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int DiasPorSemana { get; set; }
        public ICollection<EjercicioDTO> Ejercicios { get; set; }
    }
}
