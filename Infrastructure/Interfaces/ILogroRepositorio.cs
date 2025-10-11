using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface ILogroRepositorio
    {
        Task<int> CrearLogroAsync(Logro entity);
        Task<List<Logro>> ListarAsync();
        Task SetActivoAsync(int logroId, bool activo);
        Task<Logro?> ObtenerPorIdAsync(int logroId);
    }
}
