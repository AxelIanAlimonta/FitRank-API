namespace FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;

public class ActualizarEjercicioAsignadoDTO
{
    public long Id { get; set; }
    public int Orden { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public int Sesion { get; set; }
    public long RutinaId { get; set; }
    public long EjercicioId { get; set; }
    public long SocioId { get; set; }
}
