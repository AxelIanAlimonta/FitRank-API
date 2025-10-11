using AutoMapper;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class RutinaRepositorioImpl : IRutinaRepositorio
    {
        private readonly FitRankDbContext _context;
        private readonly IMapper _mapper;

        public RutinaRepositorioImpl(FitRankDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Rutina> CrearRutinaAsync(Rutina rutina)
        {
            var entity = _mapper.Map<RutinaEntity>(rutina); //rutina que persiste

            await _context.Rutinas.AddAsync(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<Rutina>(entity); //retornamos la rutina de mi dominio
        }

        public async Task<Rutina?> ObtenerRutinaAsync(int id)
        {
            var entity = await _context.Rutinas
                   .Include(r => r.Bloques)
                        .ThenInclude(b => b.Dias)
                            .ThenInclude(bd => bd.Dia)
                    .Include(r => r.Bloques)
                        .ThenInclude(b => b.Ejercicios)
                            .ThenInclude(eb => eb.Ejercicio)
                    .FirstOrDefaultAsync(r => r.Id == id);

            if (entity == null)
                return null;

            return _mapper.Map<Rutina>(entity);
        }

        public async Task<List<Rutina>> ListarRutinasAsync()
        {
            var entities = await _context.Rutinas
             .Include(r => r.Bloques)
                 .ThenInclude(b => b.Dias)
                     .ThenInclude(bd => bd.Dia)
             .Include(r => r.Bloques)
                 .ThenInclude(b => b.Ejercicios)
                     .ThenInclude(eb => eb.Ejercicio)
             .ToListAsync();

            return _mapper.Map<List<Rutina>>(entities);
        }

        public async Task<Rutina> ActualizarAsync(Rutina rutina)
        {
            var entity = await _context.Rutinas
                .Include(r => r.Bloques)
                    .ThenInclude(b => b.Dias)
                        .ThenInclude(bd => bd.Dia)
                .Include(r => r.Bloques)
                    .ThenInclude(b => b.Ejercicios)
                        .ThenInclude(eb => eb.Ejercicio)
                .FirstOrDefaultAsync(r => r.Id == rutina.Id);

            if (entity == null)
                throw new Exception("Rutina no encontrada");

            // Mapea los datos de la dominio a la que persiste
            _mapper.Map(rutina, entity);

            _context.Rutinas.Update(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<Rutina>(entity);
        }


        public async Task EliminarRutinaAsync(Rutina rutina)
        {
            var entity = await _context.Rutinas.FindAsync(rutina.Id); //rutina que persiste

            if (entity == null)
                throw new Exception("Rutina no encontrada");

            _context.Rutinas.Remove(entity); //elimino la rutina que persisten
            await _context.SaveChangesAsync();
        }

    }
}
