using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.DTOs.SocioDTOs;

public class AgregarSocioDTO
{
    //TODO: Descomentar gimansioId CUANDO SE PLANTEE LA ENTIDAD "GIMNASIO"
    //public long GimnasioId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; }

    public DateTime CuotaPagadaHasta { get; set; }
}
