using FitRank_API.Application.DTOs.EjercicioDTOs.ObtenerEjercicioDTO;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Application.DTOs.SocioDTOs;

namespace FitRank_API.Application.DTOs.EjercicioRealizadoDTOs;

public class ObtenerEjercicioRealizadoDTO
{
    public long Id { get; set; }
    public long EjercicioId { get; set; }
    public ObtenerEjercicioDTO Ejercicio { get; set; }
    public long SocioId { get; set; }
    public SocioDTO Socio { get; set; }
    public long RutinaId { get; set; }
    public ObtenerRutinaDTO Rutina { get; set; }
}
