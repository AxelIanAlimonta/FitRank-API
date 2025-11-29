using System.Diagnostics.CodeAnalysis;
using FitRank_API.Application.DTOs.EjercicioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    [ExcludeFromCodeCoverage]
    public class EjercicioCatalogoImpl : IEjercicioCatalogo
    {
        private readonly FitRankDbContext _context;

        public EjercicioCatalogoImpl(FitRankDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<EjercicioRutinaGeneradaDTO>> BuscarAsync(
               CatalogoQuery q)
        {
            var equipos = (q.EquiposPreferidos ?? Array.Empty<string>())
                .Select(s => s.Replace("EQUIPO_", "", StringComparison.OrdinalIgnoreCase))
                .Select(s => s.Trim())                           
                .Select(s => s.EndsWith("S", StringComparison.OrdinalIgnoreCase) ? s[..^1] : s) 
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var grupos = (q.Grupos ?? Array.Empty<string>()).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var evitar = (q.EvitarUsuario ?? Array.Empty<string>()).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var dolores = (q.Dolores ?? Array.Empty<string>()).ToHashSet(StringComparer.OrdinalIgnoreCase);

            IQueryable<Ejercicio> query = _context.Ejercicios
                .AsNoTracking()
                .Include(e => e.GrupoMuscular);

            if (grupos.Count > 0)
            {
                query = query.Where(e =>
                    (e.GrupoMuscular != null && grupos.Contains(e.GrupoMuscular.Nombre)) ||
                    grupos.Contains(e.Tipo.ToString()));
            }

            if (equipos.Count > 0)
            {
                query = query.Where(e => equipos.Contains(e.EquipoNecesario.ToString())); 
            }

            if (dolores.Count > 0)
            {
                query = query.Where(e => !e.ContraIndicaciones.Any(ci => dolores.Contains(ci)));
            }

            if (evitar.Count > 0)
            {
                query = query.Where(e => !e.Tags.Any(t => evitar.Contains(t)));
            }

            return await query
                .Select(e => new EjercicioRutinaGeneradaDTO(
                    e.Id,
                    e.Nombre,
                    e.GrupoMuscular != null ? e.GrupoMuscular.Nombre : e.Tipo.ToString(),
                    e.EquipoNecesario.ToString(),
                    e.Tags,
                    e.ContraIndicaciones
                ))
                .ToListAsync();
        }
    }
    }
