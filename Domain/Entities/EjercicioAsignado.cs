namespace FitRank_API.Domain.Entities;

public class EjercicioAsignado
{
    public long Id { get; set; }
    public int? Orden { get; set; }
    public string? Observaciones { get; set; } = string.Empty;
    public long RutinaId { get; set; }
    public Rutina Rutina { get; set; }
    public long EjercicioId { get; set; }
    public Ejercicio Ejercicio { get; set; }
    public long SocioId { get; set; }
    public Socio Socio { get; set; }
}
