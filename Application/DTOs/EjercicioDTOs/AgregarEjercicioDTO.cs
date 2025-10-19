namespace FitRank_API.Application.DTOs.EjercicioDTOs;

public class AgregarEjercicioDTO
{
    public string Nombre { set; get; } = string.Empty;
    public string UrlVideo { set; get; } = string.Empty;
    public long GrupoMuscularId { set; get; }
}
