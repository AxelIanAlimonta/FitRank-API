namespace FitRank_API.Domain.Entities;

public class EjercicioRealizado
{
    public long Id { set; get; }
    public long EjercicioId { set; get; }
    public Ejercicio? Ejercicio { set; get; }

    public long SocioId { set; get; }
    public Socio? Socio { set; get; }

    public long RutinaId { set; get; }
    public Rutina? Rutina { set; get; }
}
