using FitRank_API.Application.DTOs.EjercicioDTOs.ObtenerEjercicioDTO;
using FitRank_API.Application.DTOs.RutinaDTOs;

namespace FitRank_API.Application.DTOs.RutinaEjercicioDTOs;

public class ObtenerRutinaEjercicioDTO
{
    public long Id { get; set; }
    public long RutinaId { get; set; }
    public ObtenerRutinaDTO Rutina { get; set; }
    public long EjercicioId { get; set; }
    public ObtenerEjercicioDTO Ejercicio { get; set; }
    public int NumeroDeSesion { get; set; }
    public int Orden { get; set; }
}
