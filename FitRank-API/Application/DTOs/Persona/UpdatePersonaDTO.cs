namespace FitRank_API.Application.DTOs.Persona;

public class UpdatePersonaDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Edad { get; set; }
}
