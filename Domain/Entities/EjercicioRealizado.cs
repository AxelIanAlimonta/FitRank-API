namespace FitRank_API.Domain.Entities;

public class EjercicioRealizado
{
    public long Id { set; get; }

    public long EjercicioId { set; get; }
    public Ejercicio? Ejercicio { set; get; }

    public long SesionRealizadaDeEjerciciosId { set; get; }
    public SesionRealizadaDeEjercicios? SesionRealizadaDeEjercicios { set; get; }

    public ICollection<SerieRealizada>? SeriesRealizadas { set; get; }
}
