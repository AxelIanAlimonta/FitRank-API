using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IUsuarioRepository
    {
        List<Usuario> GetUsuariosConPuntuaciones();
    }
}

