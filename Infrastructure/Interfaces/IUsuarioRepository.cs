using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> GetByIdAsync(int usuarioId);
        List<Usuario> GetUsuariosConPuntuaciones();
    }
}

