namespace FitRank_API.Domain.Entities;

public class Puntaje
{
    public long Id { set; get; }
    public long SerieRealizadaId { set; get; } //FK
    public SerieRealizada? SerieRealizada { set; get; } // Propiedad de navegación
    public long SocioId { set; get; } //FK
    public Socio? Socio { set; get; } // Propiedad de navegación
    public string Motivo { set; get; }
    public DateTime Fecha { set; get; }
    public int Valor { set; get; }
}
