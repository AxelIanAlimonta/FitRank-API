namespace FitRank_API.Domain.Entities;

public class EjercicioAsignado
{
    public long Id { get; set; }
    public int? Orden { get; set; }
    public string? Observaciones { get; set; } = string.Empty;
    public int? Sesion { get; set; }

    public long RutinaId { get; set; }
    public Rutina Rutina { get; set; }
    public long EjercicioId { get; set; }
    public Ejercicio Ejercicio { get; set; }
    public long SocioId { get; set; }
    public Socio Socio { get; set; }
}





//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace FitRank_API.Domain.Entities
//{
//    public class EjercicioAsignado
//    {
//        [Key]
//        public long Id { get; set; }

//        [Required]
//        public int NumeroEjercicio { get; set; } // orden del ejercicio en la sesión

//        // Relaciones
//        [Required]
//        public long EjercicioId { get; set; }
//        [ForeignKey("EjercicioId")]
//        public Ejercicio Ejercicio { get; set; } = null!;

//        [Required]
//        public long SesionId { get; set; }
//        [ForeignKey("SesionId")]
//        public Sesion Sesion { get; set; } = null!;

//        // SerieAsignada (por ahora comentada)
//        // public long? SerieAsignadaId { get; set; }
//        // public SerieAsignada? SerieAsignada { get; set; }
//    }
//}