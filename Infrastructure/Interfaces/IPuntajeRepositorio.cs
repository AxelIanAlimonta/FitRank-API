using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IPuntajeRepositorio
    {
        Task<List<Puntaje>> ObtenerTodasAsync();
        Task<Puntaje?> ObtenerPorIdAsync(long id);
        Task<Puntaje> AgregarAsync(Puntaje rutina);
        Task<Puntaje?> ActualizarAsync(Puntaje rutina);
        Task<bool> EliminarAsync(long id);
    }
}
