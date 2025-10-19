namespace FitRank_API.Application.DTOs.RutinaDTOs;

public class AgregarRutinaDTO
{
    public string? Nombre { get; set; } = string.Empty;
    public int? Frecuencia { get; set; } = 1;
    public long DificultadId { get; set; }
}
