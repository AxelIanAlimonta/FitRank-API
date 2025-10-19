namespace FitRank_API.Application.DTOs.SerieAsignadaDTOs;

public class ActualizarSerieAsignadaDTO
{
    public long Id { get; set; }
    public int Peso { get; set; }
    public int Repeticiones { get; set; }
    public int Rir { get; set; }
    public int NroSerie { get; set; }
    public long EjercicioAsignadoId { get; set; }
}
