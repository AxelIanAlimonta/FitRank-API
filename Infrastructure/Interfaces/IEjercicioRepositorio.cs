using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;

public interface IEjercicioRepositorio
{
    Task<List<Ejercicio>> GetAllAsync();
    Task<Ejercicio?> AddAsync(Ejercicio ejercicio);
    Task UpdateAsync(Ejercicio ejercicio);
    Task DeleteAsync(long id);
    Task<Ejercicio?> GetByIdAsync(long id);


}
