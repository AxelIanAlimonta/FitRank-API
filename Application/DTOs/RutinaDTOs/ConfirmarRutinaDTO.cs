using FitRank_API.Application.DTOs.RutinaDTOs;

public class ConfirmarRutinaDTO
{
    public long SocioId { get; set; }
    public long UsuarioId { get; set; }
    public RutinaGeneradaPorIADTO Rutina { get; set; } = default!;
}
