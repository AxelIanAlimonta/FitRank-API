namespace FitRank_API.Domain.Entities;

public class Socio : Usuario
{

    public long Id { get; set; }
    public long GimnasioId { get; set; }
    public Gimnasio Gimnasio { get; set; } = null!;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; }

    public DateTime CuotaPagadaHasta { get; set; }

    //Coleccion de logros
    //Coleccion de rutinas

}
