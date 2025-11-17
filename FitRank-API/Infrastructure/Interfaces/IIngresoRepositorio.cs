using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IIngresoRepositorio
    {
        Task<IEnumerable<Ingreso>> ObtenerTodosAsync();
        Task<IEnumerable<Ingreso>> ObtenerPorGimnasioAsync(long gimnasioId);
        Task<Ingreso?> ObtenerPorIdAsync(long id);
        Task AgregarAsync(Ingreso ingreso);
        Task EliminarAsync(Ingreso ingreso);
        Task GuardarCambiosAsync();
    }
}
