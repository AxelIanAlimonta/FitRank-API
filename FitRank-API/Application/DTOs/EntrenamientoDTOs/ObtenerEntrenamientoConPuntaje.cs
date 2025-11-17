using FitRank_API.Application.DTOs.ActividadDTOs;

namespace FitRank_API.Application.DTOs.EntrenamientoDTOs
{
    public class ObtenerEntrenamientoConPuntaje
    {
        public long EntrenamientoId { get; set; }
        public List<ObtenerActividadConPuntajeDTO> Actividades { get; set; } = new List<ObtenerActividadConPuntajeDTO>();
        public double PuntosTotales { get; set; }
    }
}
