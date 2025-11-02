using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.DTOs.SocioDTOs;

public class SocioDTO
{
    public long Id { get; set; }
    public long GimnasioId { get; set; }
    public Gimnasio Gimnasio { get; set; } = null!;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;

    public string Nivel { get; set; } = string.Empty;
  

  
}
