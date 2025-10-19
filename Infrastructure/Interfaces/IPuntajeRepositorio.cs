using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IPuntajeRepositorio
    {
        Task<List<Puntaje>> ObtenerTodas();
        Task<Puntaje?> ObtenerPorId(long id);
        Task<Puntaje> Agregar(Puntaje rutina);
        Task<Puntaje?> Actualizar(Puntaje rutina);
        Task<bool> Eliminar(long id);
    }
}
