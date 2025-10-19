namespace FitRank_API.Application.DTOs.RutinaDTOs;

public class ActualizarRutinaDTO
{
    public long Id { get; set; }
    public string? Nombre { get; set; }
    public int? Frecuencia { get; set; }
    public long DificultadId { get; set; }
}
