using System.Text.Json.Serialization;

namespace FitRank_API.Domain.Entities;

public class Ejercicio
{
    public long Id { set; get; }
    public string Nombre { set; get; } = string.Empty;
    public string UrlVideo { set; get; } = string.Empty;

    public long GrupoMuscularId { set; get; }
    public GrupoMuscular? GrupoMuscular { set; get; }
}
