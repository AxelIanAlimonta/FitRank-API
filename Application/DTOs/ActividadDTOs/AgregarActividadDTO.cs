namespace FitRank_API.Application.DTOs.ActividadDTOs
{
        public class AgregarActividadDTO
        {
                public TimeSpan? Duracion { get; set; }
                public int? Repeticiones { get; set; }
                public double? Peso { get; set; }
                public double? Punto { get; set; }
                public long SerieId { get; set; }
                public long EntrenamientoId { get; set; }
                public long EjercicioAsignadoId { get; set; }
        }
}
