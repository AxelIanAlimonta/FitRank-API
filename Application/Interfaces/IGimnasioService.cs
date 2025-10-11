using FitRank_API.Application.DTOs.Logro;

namespace FitRank_API.Application.Interfaces
{
    public interface IGimnasioService
    {
        Task<IReadOnlyList<LogroDto>> ListarLogrosActivosAsync(int gimnasioId);
        Task SetEstadoLogroAsync(int gimnasioId, int logroId, bool activo);
        Task OtorgarLogroAsync(int socioId, int logroId, int gimnasioId);
    }
}
