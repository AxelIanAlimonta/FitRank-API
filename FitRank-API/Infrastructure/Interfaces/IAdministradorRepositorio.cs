using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IAdministradorRepositorio
    {
        Task<Administrador> AgregarAsync(Administrador admin);
        Task EliminarAsync(Administrador admin);
        Task <Administrador>ObtenerPorIdAsync(long id);
        Task<IEnumerable<Administrador>> ObtenerTodosAsync();

        Task ActualizarAsync(Administrador admin);

        Task<IEnumerable<Administrador>> ObtenerTodosPorGimnasio(long gimnasioId);
    }
}
