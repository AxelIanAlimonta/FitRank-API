using FitRank_API.Application.DTOs.Logro;

namespace FitRank_API.Application.Interfaces
{
    public interface ISocioService
    {
        Task<IReadOnlyList<LogroUsuarioDto>> MisLogrosAsync(int socioId, int gimnasioId);
    }
}
