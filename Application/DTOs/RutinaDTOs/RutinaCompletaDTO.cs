namespace FitRank_API.Application.DTOs.RutinaDTOs
{
    public class SerieDTO
    {
        public long Id { get; set; }
        public double? Peso { get; set; }
        public int? Repeticiones { get; set; }
        public DateTime? Duracion { get; set; }
    }

    public class EjercicioDTO
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string UrlImagen { get; set; } = string.Empty;
        public string UrlVideo { get; set; } = string.Empty;
        public  int DuracionEstimada { get; set; }
}

    public class EjercicioAsignadoDTO
    {
        public long Id { get; set; }
        public int NumeroEjercicio { get; set; }

        
        public EjercicioDTO Ejercicio { get; set; } = new();

        
        public List<SerieDTO> Series { get; set; } = new();
    }

    public class SesionDTO
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int NumeroDeSesion { get; set; }

        public List<EjercicioAsignadoDTO> EjerciciosAsignados { get; set; } = new();
    }

    public class RutinaCompletaDTO
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Activa { get; set; }

        public List<SesionDTO> Sesiones { get; set; } = new();
    }
}
