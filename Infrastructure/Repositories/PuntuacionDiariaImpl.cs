using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class PuntuacionDiariaImpl : IPuntuacionDiariaRepository
    {
        private readonly FitRankDbContext _context;
        public PuntuacionDiariaImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public Task<PuntuacionDiaria> GetByUsuarioYFechaAsync(int usuarioId, DateTime fechaHoy)
        {
            var puntuacionDiaria = _context.PuntuacionesDiarias
                .FirstOrDefaultAsync(pd => pd.UsuarioId == usuarioId && pd.Fecha.Date == fechaHoy.Date);
            return puntuacionDiaria;

        }
        public async Task ModificarPuntuacionDiaria(PuntuacionDiaria puntuacionDiaria)
        {
            var existingPuntuacion = await _context.PuntuacionesDiarias
                .FirstOrDefaultAsync(pd => pd.Id == puntuacionDiaria.Id);

            if (existingPuntuacion != null)
            {
                existingPuntuacion.Puntos = puntuacionDiaria.Puntos;
                await _context.SaveChangesAsync();
            }
        }


        public async Task RegistrarPuntuacionDiaria(PuntuacionDiaria puntuacionDiaria)
        {
            if (puntuacionDiaria == null)
                throw new ArgumentNullException(nameof(puntuacionDiaria));

            await _context.PuntuacionesDiarias.AddAsync(puntuacionDiaria);
            await _context.SaveChangesAsync();
        }

    }
}
