namespace FitRank_API.Domain.Entities;
public class Batalla
{
    public long Id { get; set; }
    public long SocioAId { get; set; }
    public long SocioBId { get; set; }
    public string Tipo { get; set; }  // "puntos", "consistencia", "miniobjetivos"
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string Estado { get; set; } // "Activa", "Finalizada"
}
