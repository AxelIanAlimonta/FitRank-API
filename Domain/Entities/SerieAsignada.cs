namespace FitRank_API.Domain.Entities;

public class SerieAsignada
{
    public long Id { get; set; }
    public int Peso { get; set; }
    public int Repeticiones { get; set; }
    public int Rir { get; set; }
    public int NroSerie { get; set; }
    public long EjercicioAsignadoId { get; set; }
    public EjercicioAsignado EjercicioAsignado { get; set; }
}
