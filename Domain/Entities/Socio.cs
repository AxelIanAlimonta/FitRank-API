namespace FitRank_API.Domain.Entities;

public class Socio : Usuario
{
    public long GimnasioId { get; set; }
    public Gimnasio Gimnasio { get; set; } = null!;
    public DateTime FechaRegistro { get; set; }
    public ICollection<Puntaje> Puntajes { get; set; } = new List<Puntaje>();

    //Coleccion de logros
    //Coleccion de rutinas

}
