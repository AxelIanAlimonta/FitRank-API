using FitRank_API.Application.DTOs.Logro;
namespace FitRank_API.Application.Interfaces
{
    public interface ILogroService
    {
        Task<int> CrearLogroAsync(LogroCreateDto logroDto);
        Task<IReadOnlyList<LogroDto>> ListarAsync();
        Task SetActivoAsync(int logroId, bool activo);
        Task<LogroDto?> ObtenerPorIdAsync(int logroId);
    }
}
