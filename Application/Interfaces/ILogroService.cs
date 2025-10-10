using FitRank_API.Application.DTOs.Logro;
namespace FitRank_API.Application.Interfaces
{
    public interface ILogroService
    {
        Task<IReadOnlyList<LogroDto>> ListarActivosAsync(CancellationToken ct = default);
        Task<IReadOnlyList<LogroUsuarioDto>> MisLogrosAsync(int socioId, CancellationToken ct = default);
        Task OtorgarSiNoExisteAsync(int socioId, int logroId, CancellationToken ct = default);
        Task SetActivoAsync(int logroId, bool activo, CancellationToken ct = default);
    }
}
