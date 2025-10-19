namespace FitRank_API.Domain.Entities;

public class Puntaje
{
    public long Id { set; get; }
    public long SerieRealizadaId { set; get; } //FK
    public string Motivo { set; get; }
    public DateTime Fecha { set; get; }
    public int Valor { set; get; }
}
