using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Entities;

public class SolicitudRutinaProfesor
{
    public long Id { get; set; }

    public long SocioId { get; set; }
    public Socio Socio { get; set; } = null!;

    public long? ProfesorId { get; set; }
    public Profesor? Profesor { get; set; }

    public string NombreSocio { get; set; } = string.Empty;

    public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;

    public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;
    public DateTime? FechaResolucion { get; set; }

    public string? MensajeSocio { get; set; }
    public string? MensajeProfesor { get; set; }

    public long? RutinaId { get; set; }
    public Rutina? Rutina { get; set; }

    // Datos solicitados (como en la IA)
    public int Edad { get; set; }
    public double PesoKg { get; set; }
    public double AlturaCm { get; set; }
    public string Nivel { get; set; } = string.Empty;
    public int SesionesPorSemana { get; set; }
    public int MinutosPorSesion { get; set; }
    public string Objetivo { get; set; } = string.Empty;
    public int CalidadAlimentacion { get; set; }
    public int HorasSuenio { get; set; }

    // Screening
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
