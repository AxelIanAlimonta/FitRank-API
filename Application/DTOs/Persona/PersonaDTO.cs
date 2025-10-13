using System.ComponentModel.DataAnnotations;

public class PersonaDTO
{

    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; }

    public int Edad { set; get; }
}