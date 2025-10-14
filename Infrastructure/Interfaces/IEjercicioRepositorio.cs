using FitRank_API.Application.DTOs.Rutina;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IEjercicioRepositorio
    {
        Task<List<Ejercicio>> ListarEjerciciosAsync();
    }
}
