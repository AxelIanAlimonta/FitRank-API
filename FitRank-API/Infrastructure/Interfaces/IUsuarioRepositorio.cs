using System.Linq.Expressions;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IUsuarioRepositorio
    {
        Task<bool> ExistePorEmailAsync(string email);
        Task<Usuario?> ObtenerPorEmailAsync(string email);
        Task<Usuario?> ObtenerPorIdAsync(long id);
        Task<Usuario?> ObtenerPorTokenActivacionAsync(string token);
        Task <Usuario>AgregarAsync(Usuario usuario);
        Task <Usuario?>ActualizarAsync(Usuario usuario);
        Task EliminarAsync(Usuario usuario);
        Task<List<Usuario>> ObtenerTodosAsync();
        
        Task<Usuario?> ObtenerPorCondicionAsync(Expression<Func<Usuario, bool>> predicado);

        Task<Socio?> ObtenerSocioConGimnasioPorIdAsync(long id);

        Task<List<Socio>> ObtenerSociosActivosAsync();




    }
}
