using FitRank_API.Application.DTOs.ActividadDTOs;

namespace FitRank_API.Application.DTOs.EntrenamientoDTOs
{
    public class RegistrarEntrenamientoDTO
    {
        public long SocioId { get; set; }
        public List<RegistrarActividadDTO> Actividades { get; set; } = new List<RegistrarActividadDTO>();
    }
}
