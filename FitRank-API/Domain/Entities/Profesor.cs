    namespace FitRank_API.Domain.Entities
    {
        public class Profesor : Usuario
        {
            public string Matricula { get; set; } = string.Empty;
            public double Sueldo { get; set; }

  
            public long? GimnasioId { get; set; }
            public Gimnasio Gimnasio { get; set; } = null!;

    
       

            public ICollection<SolicitudRutinaProfesor>? Solicitudes { get; set; } = new List<SolicitudRutinaProfesor>();
        }
    }

