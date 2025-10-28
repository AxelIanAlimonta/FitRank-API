namespace FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;

public class AgregarEjercicioAsignadoDTO
{
    public int Orden { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public long EjercicioId { get; set; }
    public long SesionId { get; set; }
    //public long SocioId { get; set; } // si seguís queriendo registrar el socio
}
