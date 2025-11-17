using FitRank_API.Domain.Entities;

using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories;


    public class IngresoRepositorio : IIngresoRepositorio
    {
        private readonly FitRankDbContext _context;

        public IngresoRepositorio(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ingreso>> ObtenerTodosAsync()
        {
            return await _context.Ingresos
                .Include(i => i.Usuario)
                .Include(i => i.Gimnasio)
                .OrderByDescending(i => i.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ingreso>> ObtenerPorGimnasioAsync(long gimnasioId)
        {
            return await _context.Ingresos
                .Where(i => i.GimnasioId == gimnasioId)
                .Include(i => i.Usuario)
                .Include(i => i.Gimnasio)
                .OrderByDescending(i => i.Fecha)
                .ToListAsync();
        }

        public async Task<Ingreso?> ObtenerPorIdAsync(long id)
        {
            return await _context.Ingresos
                .Include(i => i.Usuario)
                .Include(i => i.Gimnasio)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task AgregarAsync(Ingreso ingreso)
        {
            await _context.Ingresos.AddAsync(ingreso);
        }

        public async Task EliminarAsync(Ingreso ingreso)
        {
            _context.Ingresos.Remove(ingreso);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

