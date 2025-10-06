using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IEjercicioRealizadoRepository
    {
        Task AddEjercicioRealizado(EjercicioRealizado ejercicioRealizado);
        Task<IEnumerable<EjercicioRealizado>> GetByUsuarioAsync(int usuarioId);
    }
}
