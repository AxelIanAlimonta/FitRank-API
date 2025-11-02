using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class ConfiguracionGrupoMuscularImpl : IConfiguracionGrupoMuscularRepositorio
    {
        private readonly FitRankDbContext _context;
        public ConfiguracionGrupoMuscularImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<List<ConfiguracionGrupoMuscular>> ObtenerTodosAsync()
        {
            return await _context.ConfiguracionesGrupoMuscular.ToListAsync();
        }

        public async Task<ConfiguracionGrupoMuscular?> ObtenerPorIdAsync(long id)
        {
            return await _context.ConfiguracionesGrupoMuscular.FindAsync(id);
        }

        public async Task<ConfiguracionGrupoMuscular?> AgregarAsync(ConfiguracionGrupoMuscular configuracionGrupoMuscular)
        {
            var resultado = await _context.ConfiguracionesGrupoMuscular.AddAsync(configuracionGrupoMuscular);
            await _context.SaveChangesAsync();
            return resultado.Entity;
        }

        public async Task<ConfiguracionGrupoMuscular?> ActualizarAsync(ConfiguracionGrupoMuscular configuracionGrupoMuscular)
        {
            var existente = await _context.ConfiguracionesGrupoMuscular.FindAsync(configuracionGrupoMuscular.Id);
            if (existente == null)
            {
                return null;
            }
            existente.MultiplicadorPeso = configuracionGrupoMuscular.MultiplicadorPeso;
            existente.MultiplicadorRepeticiones = configuracionGrupoMuscular.MultiplicadorRepeticiones;
            existente.GrupoMuscularId = configuracionGrupoMuscular.GrupoMuscularId;
            existente.FactorProgresion = configuracionGrupoMuscular.FactorProgresion;
            await _context.SaveChangesAsync();
            return existente;
        }

        public async Task EliminarAsync(long id)
        {
            var configuracionGrupoMuscular = await _context.ConfiguracionesGrupoMuscular.FindAsync(id);
            if (configuracionGrupoMuscular != null)
            {
                _context.ConfiguracionesGrupoMuscular.Remove(configuracionGrupoMuscular);
                await _context.SaveChangesAsync();
            }
        }
    }
}
