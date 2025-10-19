using FitRank_API.Application.DTOs.DificultadDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.DTOs.RutinaDTOs;

public class ObtenerRutinaDTO
{
    public long Id { get; set; }
    public string? Nombre { get; set; }
    public int? Frecuencia { get; set; }
    public long DificultadId { get; set; }
    public DificultadDTO Dificultad { get; set; } = new DificultadDTO();

}
