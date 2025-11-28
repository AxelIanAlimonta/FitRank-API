using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Interfaces
{
    public interface IDiaDeLaSemanaRepositorio
    {
        Task<List<DiaDeLaSemana>> ObtenerTodosLosDiasDeLaSemanaAsync();

        Task<DiaDeLaSemana?> ObtenerDiaDeLaSemanaPorIdAsync(long id);

        Task<DiaDeLaSemana> AgregarDiaDeLaSemanaAsync(DiaDeLaSemana diaDeLaSemana);

        Task<DiaDeLaSemana?> ActualizarDiaDeLaSemanaAsync(DiaDeLaSemana diaDeLaSemana);

        Task<bool> EliminarDiaDeLaSemanaAsync(long id);
    }
}
