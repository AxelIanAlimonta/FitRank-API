using FitRank_API.Application.DTOs.ActividadDTOs;

namespace FitRank_API.Application.DTOs.EntrenamientoDTOs
{
    public class RegistrarEntrenamientoConActividadesDTO
    {
        public long SocioId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public TimeSpan? Duracion { get; set; }
        public List<AgregarActividadDTO> Actividades { get; set; } = new List<AgregarActividadDTO>();
    }
}
