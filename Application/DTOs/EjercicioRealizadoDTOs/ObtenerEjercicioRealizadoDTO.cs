using FitRank_API.Application.DTOs.EjercicioDTOs;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Application.DTOs.SocioDTOs;

namespace FitRank_API.Application.DTOs.EjercicioRealizadoDTOs;

public class ObtenerEjercicioRealizadoDTO
{
    public long Id { get; set; }
    public long EjercicioId { get; set; }
    public EjercicioDTO Ejercicio { get; set; }
    public long SocioId { get; set; }
    public SocioDTO Socio { get; set; }
    public long RutinaId { get; set; }
    public ObtenerRutinaDTO Rutina { get; set; }
}
