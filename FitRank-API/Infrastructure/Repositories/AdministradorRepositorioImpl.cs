using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class AdministradorRepositorioImpl : IAdministradorRepositorio
    {
        private readonly FitRankDbContext _context;

        public AdministradorRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<Administrador> AgregarAsync(Administrador admin)
        {
            await _context.Administradores.AddAsync(admin);
            await _context.SaveChangesAsync();
            return admin;
        }

        public async Task EliminarAsync(Administrador admin)
        {
            _context.Administradores.Remove(admin);
            await _context.SaveChangesAsync();
        }


        public async Task<Administrador?> ObtenerPorIdAsync(long id)
        {
            return await _context.Administradores
       .AsNoTracking()
       .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Administrador>> ObtenerTodosAsync()
        {
            return await _context.Administradores
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<Gimnasio?> ObtenerPorAdministradorIdAsync(long administradorId)
        {
            return await _context.Gimnasios
                .Include(g => g.Administrador)
                .FirstOrDefaultAsync(g => g.AdministradorId == administradorId);
        }



        public async Task ActualizarAsync(Administrador admin)
        {
            _context.Administradores.Update(admin);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Administrador>> ObtenerTodosPorGimnasio(long gimnasioId)
        {
            return await _context.Administradores
                .Where(a => a.GimnasioId == gimnasioId)
                .ToListAsync();
        }

    }
}
