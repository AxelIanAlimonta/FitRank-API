namespace FitRank_API.Application.DTOs.EntrenamientoDTOs
{
    public class EntrenamientoHistorialDTO
    {
        public long IdEntrenamiento { get; set; }
        public DateTime Fecha { get; set; }
        public string NombreSesion { get; set; } = string.Empty;
        public TimeSpan? Duracion { get; set; }
        public double PuntosTotales { get; set; }
        public List<ActividadHistorialDTO> Actividades { get; set; } = new();
    }

    public class ActividadHistorialDTO
    {
        public long IdActividad { get; set; }
        public long IdEjercicioAsignado { get; set; }
        public string NombreEjercicio { get; set; } = string.Empty;
        public string? UrlImagen { get; set; }
        public int? Repeticiones { get; set; }
        public double? Peso { get; set; }
        public double? Punto { get; set; }
        public List<ProgresoEjercicioDTO> ProgresoHistorico { get; set; } = new();
    }

    public class ProgresoEjercicioDTO
    {
        public DateTime Fecha { get; set; }
        public double? Peso { get; set; }
        public int? Repeticiones { get; set; }
    }
}
