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

        public async Task AgregarUsuario(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario> GetByIdAsync(int usuarioId)
        {
           return await _context.Usuarios.FindAsync(usuarioId); 

        }

        public List<Usuario> GetUsuariosConPuntuaciones()
        {
           var usuarios = _context.Usuarios
                .Where(u => u.puntuacionesDiarias.Any())
                .ToList();
            return usuarios;
        }
    }
}
