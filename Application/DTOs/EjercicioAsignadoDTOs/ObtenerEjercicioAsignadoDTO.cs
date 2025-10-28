using FitRank_API.Application.DTOs.EjercicioDTOs;
using FitRank_API.Application.DTOs.EjercicioDTOs.ObtenerEjercicioDTO;

namespace FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;

public class ObtenerEjercicioAsignadoDTO
{
    public long Id { get; set; }
    public int Orden { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public long EjercicioId { get; set; }
    public ObtenerEjercicioDTO? Ejercicio { get; set; }
    public long SesionId { get; set; }
}
