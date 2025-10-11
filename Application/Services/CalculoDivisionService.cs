using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Application.Services
{
    public class CalculoDivisionService
    {
        private readonly FitRankDbContext _context;

        public CalculoDivisionService(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<string> CalcularDivisionAsync(Usuario usuario)
        {
            double puntosTotales = usuario.EjerciciosRealizados.Sum(e => e.PuntosObtenidos);

            var divisiones = await _context.ConfiguracionesDivisiones.ToListAsync();

            var divisionActual = divisiones
                .FirstOrDefault(d =>
                    puntosTotales >= d.PuntosMinimos &&
                    (d.PuntosMaximos == 0 || puntosTotales < d.PuntosMaximos));

            return divisionActual?.Nombre ?? "Sin división";
        }

        public async Task<string> CalcularDivisionPorGrupoAsync(Usuario usuario, GrupoMuscular grupo)
        {
            var ejerciciosDelGrupo = usuario.EjerciciosRealizados
                .Where(er => er.Ejercicio.GrupoMuscular == grupo);

            double puntosTotales = ejerciciosDelGrupo.Sum(er => er.PuntosObtenidos);

            var divisiones = await _context.ConfiguracionesDivisiones.ToListAsync();

            var division = divisiones
                .FirstOrDefault(d => puntosTotales >= d.PuntosMinimos &&
                                     (d.PuntosMaximos == null || puntosTotales <= d.PuntosMaximos));

            return division?.Nombre ?? "Sin división";
        }

        public async Task<Dictionary<GrupoMuscular, string>> ObtenerDivisionesPorUsuarioAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.EjerciciosRealizados)
                .ThenInclude(er => er.Ejercicio)
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null) throw new Exception("Usuario no encontrado");

            var divisionService = new CalculoDivisionService(_context);

            var divisionesPorGrupo = new Dictionary<GrupoMuscular, string>();
            foreach (GrupoMuscular gm in Enum.GetValues(typeof(GrupoMuscular)))
            {
                divisionesPorGrupo[gm] = await divisionService.CalcularDivisionPorGrupoAsync(usuario, gm);
            }

            return divisionesPorGrupo;
        }


    }
}
