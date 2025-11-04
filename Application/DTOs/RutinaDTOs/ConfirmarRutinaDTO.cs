using FitRank_API.Application.DTOs.RutinaDTOs;

public class ConfirmarRutinaDTO
{

    public long SocioId { get; set; }
    public long UsuarioId { get; set; }
    public RutinaGeneradaPorIADTO Rutina { get; set; }

    public ConfirmarRutinaDTO(long SocioId, long UsuarioId, RutinaGeneradaPorIADTO rutina)
    {
        this.SocioId = SocioId;
        this.UsuarioId = UsuarioId;
        this.Rutina = rutina;
    }
}
