using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class JornadaRepositorioImpl : IJornadaRepositorio
    {
        private readonly FitRankDbContext _context;
        public JornadaRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<List<Jornada>> ObtenerTodasLasJornadasAsync()
        {
            return await _context.Jornadas
                .Include(j => j.Profesor)
                .Include(j => j.DiaDeLaSemana)
                .ToListAsync();
        }

        public async Task<Jornada?> ObtenerJornadaPorIdAsync(long id)
        {
            return await _context.Jornadas
                .Include(j => j.Profesor)
                .Include(j => j.DiaDeLaSemana)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<Jornada> AgregarJornadaAsync(Jornada nuevaJornada)
        {
            _context.Jornadas.Add(nuevaJornada);
            await _context.SaveChangesAsync();
            return nuevaJornada;
        }

        public async Task<Jornada?> ActualizarJornadaAsync(Jornada jornadaActualizada)
        {
            var jornadaExistente = await _context.Jornadas.FindAsync(jornadaActualizada.Id);
            if (jornadaExistente == null)
            {
                return null;
            }
            jornadaExistente.HoraInicio = jornadaActualizada.HoraInicio;
            jornadaExistente.HoraFin = jornadaActualizada.HoraFin;
            jornadaExistente.ProfesorId = jornadaActualizada.ProfesorId;
            jornadaExistente.DiaDeLaSemanaId = jornadaActualizada.DiaDeLaSemanaId;

            await _context.SaveChangesAsync();
            return jornadaExistente;
        }

        public async Task<bool> EliminarJornadaAsync(long id)
        {
            var jornadaExistente = await _context.Jornadas.FindAsync(id);
            if (jornadaExistente == null)
            {
                return false;
            }
            _context.Jornadas.Remove(jornadaExistente);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
