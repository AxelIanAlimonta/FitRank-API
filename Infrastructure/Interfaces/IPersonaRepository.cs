using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IPersonaRepository
    {
        Task<List<Persona>> GetAllAsync();
        Task AddAsync(Persona persona);
        Task UpdateAsync(Persona persona);
        Task DeleteAsync(long id);
        Task<Persona?> GetByIdAsync(long id);
    }
}
