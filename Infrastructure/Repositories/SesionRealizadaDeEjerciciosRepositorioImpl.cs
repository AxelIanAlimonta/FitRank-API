using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class SesionRealizadaDeEjerciciosRepositorioImpl : ISesionRealizadaDeEjerciciosRepositorio
    {
        private readonly FitRankDbContext _context;

        public SesionRealizadaDeEjerciciosRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<List<SesionRealizadaDeEjercicios>> ObtenerTodosAsync()
        {
            return await _context.SesionRealizadaDeEjercicios.ToListAsync();
        }

        public async Task<SesionRealizadaDeEjercicios?> ObtenerPorIdAsync(long id)
        {
            return await _context.SesionRealizadaDeEjercicios.FindAsync(id);
        }
        public async Task<SesionRealizadaDeEjercicios?> AgregarAsync(SesionRealizadaDeEjercicios sesion)
        {
            var resultado = await _context.SesionRealizadaDeEjercicios.AddAsync(sesion);
            await _context.SaveChangesAsync();
            return resultado.Entity;
        }
        public async Task<SesionRealizadaDeEjercicios?> ActualizarAsync(SesionRealizadaDeEjercicios sesion)
        {
            var existente = await _context.SesionRealizadaDeEjercicios.FindAsync(sesion.Id);
            if (existente == null)
            {
                return null;
            }
            existente.Fecha = sesion.Fecha;
            existente.Duracion = sesion.Duracion;
            existente.NumeroDeSesion = sesion.NumeroDeSesion;
            await _context.SaveChangesAsync();
            return existente;
        }

        public async Task EliminarAsync(long id)
        {
            var sesion = await _context.SesionRealizadaDeEjercicios.FindAsync(id);
            if (sesion != null)
            {
                _context.SesionRealizadaDeEjercicios.Remove(sesion);
                await _context.SaveChangesAsync();
            }
        }
    }
}
