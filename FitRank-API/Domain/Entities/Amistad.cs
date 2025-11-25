using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;

namespace FitRank_API.Domain.Entities;

public class Amistad
{
    public long Id { get; set; }

    public long SocioId1 { get; set; }
    public long SocioId2 { get; set; }

    public EstadoAmistad Estado { get; set; }
    public long SolicitanteId { get; set; }

    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }

    public Socio Socio1 { get; set; } = null!;
    public Socio Socio2 { get; set; } = null!;
    public Socio Solicitante { get; set; } = null!;
}
