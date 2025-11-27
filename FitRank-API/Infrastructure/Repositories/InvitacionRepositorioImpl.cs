using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class InvitacionRepositorioImpl : IInvitacionRepositorio
    {
        private readonly FitRankDbContext _context;

        public InvitacionRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<Invitacion?> ActualizarAsync(Invitacion invitacion)
        {
            var existente = await _context.Invitaciones.FindAsync(invitacion.Id);
            if (existente == null)
            {
                return null;
            }
            existente.Estado = invitacion.Estado;
            existente.MetodoPago = invitacion.MetodoPago;
            existente.MpPaymentId = invitacion.MpPaymentId;
            _context.Invitaciones.Update(existente);
            await _context.SaveChangesAsync();
            return existente;

        }
        public async Task<Invitacion> AgregarAsync(Invitacion invitacion)
        {
            var existente = await _context.Invitaciones.FindAsync(invitacion.Id);
            if (existente != null)
            {
                return existente;
            }
            var resultado = await _context.Invitaciones.AddAsync(invitacion);
            await _context.SaveChangesAsync();
            return invitacion;

        }

        public async Task<bool> Eliminar(long id)
        {
            var existente = await _context.Invitaciones.FindAsync(id);
            if (existente == null)
            {
                return false;
            }
            _context.Invitaciones.Remove(existente);
            await _context.SaveChangesAsync();
            return true;


        }

        public async Task<Invitacion?> ObtenerPorIdAsync(long id)
        {
            return await _context.Invitaciones
                .Include(i => i.Gimnasio)
                .FirstOrDefaultAsync(i => i.Id == id);
        }



        public async Task<Invitacion?> ObtenerPorIdYEstadoAsync(long id, string estado)
        {
            return await _context.Invitaciones
                .FirstOrDefaultAsync(i => i.Id == id && i.Estado == estado);
        }

        public async Task<List<Invitacion>> ObtenerTodasAsync(long gimnasioId)
        {
            return await _context.Invitaciones
                .Where(i => i.GimnasioId == gimnasioId)
                .Include(i => i.Usuario)
                .OrderByDescending(i => i.CreadaEn)
                .ToListAsync();
        }

        public async Task<Invitacion?> ObtenerPorEmailAsync(string email)
        {
            return await _context.Invitaciones
                .Include(i => i.Gimnasio)
                .Include(i => i.Usuario)
                .FirstOrDefaultAsync(i => i.Email == email);
        }

    }
}
