namespace FitRank_API.Domain.Entities;

public class Rutina
{
    public long Id { get; set; }
    public string? Nombre { get; set; } = string.Empty;
    public int? Frecuencia { get; set; } = 1;

    public long DificultadId { get; set; }
    public Dificultad Dificultad { get; set; } = new Dificultad();

}
