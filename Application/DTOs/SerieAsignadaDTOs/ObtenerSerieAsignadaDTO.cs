using FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;

namespace FitRank_API.Application.DTOs.SerieAsignadaDTOs;

public class ObtenerSerieAsignadaDTO
{
    public long Id { get; set; }
    public int Peso { get; set; }
    public int Repeticiones { get; set; }
    public int Rir { get; set; }
    public int NroSerie { get; set; }
    public long EjercicioAsignadoId { get; set; }
    public ObtenerEjercicioAsignadoDTO EjercicioAsignado { get; set; }
}
