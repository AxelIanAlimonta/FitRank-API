namespace FitRank_API.Application.DTOs.SolicitudDTO
{
    public class CrearSolicitudRutinaProfesorDTO
    {
        public string? MensajeSocio { get; set; }
        public string NombreSocio { get; set; }
        public int Edad { get; set; }
        public double PesoKg { get; set; }
        public double AlturaCm { get; set; }
        public string Nivel { get; set; } = string.Empty;
        public int SesionesPorSemana { get; set; }
        public int MinutosPorSesion { get; set; }
        public string Objetivo { get; set; } = string.Empty;
        public int CalidadAlimentacion { get; set; }
        public int HorasSuenio { get; set; }

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
        public int FrecuenciaCardiacaReposo { get; set; }
    }

}
