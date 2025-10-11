using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IUsuarioRepository
    {
        Task AgregarUsuario(Usuario usuario);
        Task<Usuario> GetByIdAsync(int usuarioId);
        List<Usuario> GetUsuariosConPuntuaciones();
    }
}

