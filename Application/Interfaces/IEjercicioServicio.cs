using FitRank_API.Application.DTOs.Rutina;

namespace FitRank_API.Application.Interfaces
{
    public interface IEjercicioServicio
    {
        Task<List<EjercicioDTO>> ListarEjerciciosAsync();
    }
}
