namespace FitRank_API.Domain.Entities;

public class RutinaEjercicio
{
    public long Id { get; set; }

    public long RutinaId { get; set; }
    public Rutina Rutina { get; set; }

    public long EjercicioId { get; set; }
    public Ejercicio Ejercicio { get; set; }

    public int NumeroDeSesion { get; set; }
    public int Orden { get; set; }
}
