namespace FitRank_API.Application.DTOs.RutinaEjercicioDTOs;

public class AgregarRutinaEjercicioDTO
{
    public long RutinaId { get; set; }
    public long EjercicioId { get; set; }
    public int NumeroDeSesion { get; set; }
    public int Orden { get; set; }
}
