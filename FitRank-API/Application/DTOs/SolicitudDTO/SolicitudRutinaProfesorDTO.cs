namespace FitRank_API.Application.DTOs.SolicitudDTO
{
    public class SolicitudRutinaProfesorDTO
    {
        public long Id { get; set; }
        public long SocioId { get; set; }
        public string NombreSocio { get; set; } = string.Empty;
        public long? ProfesorId { get; set; }
        public string? NombreProfesor { get; set; }
        public string Estado { get; set; } = string.Empty;

        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public string? MensajeSocio { get; set; }
        public string? MensajeProfesor { get; set; }
        public long? RutinaId { get; set; }

        // Datos del socio (opcional mostrarlos)
        public int Edad { get; set; }
        public double PesoKg { get; set; }
        public double AlturaCm { get; set; }
        public string Nivel { get; set; } = string.Empty;
        public int SesionesPorSemana { get; set; }
        public int MinutosPorSesion { get; set; }
        public string Objetivo { get; set; } = string.Empty;

        // 🩺 Screening
        public bool DolorLumbar { get; set; }
        public bool DolorRodilla { get; set; }
        public bool DolorHombro { get; set; }
        public bool CirugiaReciente { get; set; }
        public bool Sincope { get; set; }
        public bool Embarazo { get; set; }
        public bool Hipertension { get; set; }
        public bool HipertensionControlada { get; set; }
        public bool Diabetes { get; set; }
        public bool DolorToracico { get; set; }
    }

}
