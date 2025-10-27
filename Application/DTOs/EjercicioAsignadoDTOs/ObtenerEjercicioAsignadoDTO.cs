using FitRank_API.Application.DTOs.EjercicioDTOs.ObtenerEjercicioDTO;
using FitRank_API.Application.DTOs.RutinaDTOs;

namespace FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;

public class ObtenerEjercicioAsignadoDTO
{
    public long Id { get; set; }
    public int Orden { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public long RutinaId { get; set; }
    public int Sesion { get; set; }
    public ObtenerRutinaDTO? Rutina { get; set; }
    public long EjercicioId { get; set; }
    public ObtenerEjercicioDTO? Ejercicio { get; set; }
    public long SocioId { get; set; }
}
