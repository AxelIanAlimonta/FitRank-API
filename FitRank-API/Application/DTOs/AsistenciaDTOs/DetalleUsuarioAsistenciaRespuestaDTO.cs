using FitRank_API.Application.DTOs.SocioDTOs;

namespace FitRank_API.Application.DTOs.Asistencia
{
    public class DetalleUsuarioAsistenciaRespuestaDTO
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public SocioDTO? Socio { get; set; } = null;
        public List<AsistenciaDetalleUsuarioDTO> Asistencias { get; set; } = new();
    }
}
