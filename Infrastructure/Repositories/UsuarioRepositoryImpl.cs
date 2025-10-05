using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;

namespace FitRank_API.Infrastructure.Repositories
{
    public class UsuarioRepositoryImpl : IUsuarioRepository
    {
        private readonly FitRankDbContext _context;
        public UsuarioRepositoryImpl(FitRankDbContext context)
        {
            _context = context;
        }
        public List<Usuario> GetUsuariosConPuntuaciones()
        {
           var usuarios = _context.Usuarios
                .Where(u => u.PuntuacionesDiarias.Any())
                .ToList();
            return usuarios;
        }
    }
}
