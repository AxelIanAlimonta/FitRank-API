namespace FitRank_API.Domain.Entities;

public class Socio : Usuario
{
    public long? GimnasioId { get; set; }
    public Gimnasio Gimnasio { get; set; } = null!;
    public DateTime FechaRegistro { get; set; }

    public double Altura { get; set; }
    public double Peso { get; set; }

    public string Nivel { get; set; }

    public double Puntaje { get; set; }

    public ICollection<MedidaCorporal>? MedidasCorporales { get; set; } = new List<MedidaCorporal>();
    public ICollection<Foto>? FotosProgreso { get; set; } = new List<Foto>();

    public ICollection<Entrenamiento> Entrenamientos { get; set; }
}


