namespace FitRank_API.Application.DTOs.LogroDTOs;

public class ActualizarLogroDTO
{
    public long Id { get; set; }
    public string? NombreClave { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public string? Categoria { get; set; }
    public string? Imagen { get; set; }
    public int? Puntos { get; set; }
}
