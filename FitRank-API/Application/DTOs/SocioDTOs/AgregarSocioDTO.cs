using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.DTOs.SocioDTOs;

public class AgregarSocioDTO
{
  
    public string NombreUsuario { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Sexo { get; set; } = string.Empty;
    public string FotoDePerfil { get; set; } = string.Empty;

    
    public long? GimnasioId { get; set; } 
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public double Altura { get; set; }
    public double Peso { get; set; }
    public string Nivel { get; set; } = string.Empty;
}
