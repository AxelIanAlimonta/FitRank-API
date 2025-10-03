using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Application.DTOs.Persona;

public class PersonaDTO
{

    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; }

    public int Edad { set; get; }
}
