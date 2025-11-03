
using FitRank_API.Application.DTOs.UsuarioDTOs;

namespace FitRank_API.Application.DTOs.Asistencia
{
    public class QrValidationResponseDTO
    {

            public bool Valido { get; set; }
            public string Mensaje { get; set; } = string.Empty;
            public UsuarioAuthDTO? User { get; set; }
            public long? AsistenciaId { get; set; }
        public DateTime? ValidoHasta { get; set; }

    }
}
