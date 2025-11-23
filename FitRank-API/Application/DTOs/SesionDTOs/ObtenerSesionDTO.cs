namespace FitRank_API.Application.DTOs.SesionDTOs;

public class ObtenerSesionDTO
{
    public long Id { get; set; }
    public int NumeroDeSesion { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public long RutinaId { get; set; }
    public string RutinaNombre { get; set; } = string.Empty;
}
