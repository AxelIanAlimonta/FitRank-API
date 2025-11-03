namespace FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;

public class ActualizarEjercicioAsignadoDTO
{
    public long Id { get; set; }
    public int NumeroEjercicio { get; set; }
    public long EjercicioId { get; set; }
    public long SesionId { get; set; }
}