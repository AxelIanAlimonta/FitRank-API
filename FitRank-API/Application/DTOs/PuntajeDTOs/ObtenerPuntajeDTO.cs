using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.DTOs.PuntajeDTOs;

public class ObtenerPuntajeDTO
{
    public long Id { set; get; }
    public string Motivo { set; get; }
    public DateTime Fecha { set; get; }
    public int Valor { set; get; }
    public long SocioId { set; get; }
}
