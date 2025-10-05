using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Domain.Entities;

public class Persona
{
    public long Id { get; set; }
    [Required]
    public string Nombre { get; set; }
    public int Edad { get; set; }


}
