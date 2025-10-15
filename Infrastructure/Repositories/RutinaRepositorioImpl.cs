using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class RutinaRepositorioImpl: IRutinaRepository
    {
        private readonly FitRankDbContext _context;
     

        public RutinaRepositorioImpl(FitRankDbContext context)
        {
            _context = context;
         
        }
        //RUTINAS
        public async Task<Rutina> CrearRutinaAsync(Rutina rutina)
        {
            _context.Rutinas.Add(rutina);
            await _context.SaveChangesAsync();
            return rutina;
        }

        public async Task<List<Rutina>> ListarRutinasAsync()
        {
            return await _context.Rutinas
                .Include(b => b.Ejercicios)
                .ToListAsync();
        }

        public async Task<List<Rutina>> ListarRutinasPorUsuarioAsync(int usuarioId)
        {
            return await _context.Rutinas
                .Where(r => r.UsuarioId == usuarioId)
                .Include(b => b.Ejercicios)
                .ToListAsync();
        }

        public async Task<Rutina> ObtenerRutinaPorIdAsync(int rutinaId)
        {
            return await _context.Rutinas
                        .Include(r => r.Ejercicios)
                        .FirstOrDefaultAsync(r => r.Id == rutinaId);
        }

        public async Task<Rutina> ActualizarRutinaAsync(Rutina rutina)
        {
            var rutinaExistente = await _context.Rutinas
                .Include(r => r.Ejercicios)
                .FirstOrDefaultAsync(r => r.Id == rutina.Id);

            if (rutinaExistente == null)
                return null;

            rutinaExistente.Nombre = rutina.Nombre;
            rutinaExistente.FechaInicio = rutina.FechaInicio;
            rutinaExistente.FechaFin = rutina.FechaFin;
            rutinaExistente.DiasPorSemana = rutina.DiasPorSemana;

            // Eliminar ejercicios que ya no están en la rutina recibida
            var ejerciciosAEliminar = rutinaExistente.Ejercicios
                .Where(e => !rutina.Ejercicios.Any(re => re.Id == e.Id))
                .ToList();

            foreach (var ejercicio in ejerciciosAEliminar)
            {
                rutinaExistente.Ejercicios.Remove(ejercicio);
                _context.Ejercicios.Remove(ejercicio);
            }

            // Actualizar o agregar ejercicios
            foreach (var ejercicio in rutina.Ejercicios)
            {
                var ejercicioExistente = rutinaExistente.Ejercicios
                    .FirstOrDefault(e => e.Id == ejercicio.Id);

                if (ejercicioExistente != null)
                {
                    // Actualizar propiedades
                    ejercicioExistente.Nombre = ejercicio.Nombre;
                    ejercicioExistente.Series = ejercicio.Series;
                    ejercicioExistente.Repeticiones = ejercicio.Repeticiones;
                    ejercicioExistente.Peso = ejercicio.Peso;
                    ejercicioExistente.MaquinaId = ejercicio.MaquinaId;
                }
                else
                {
                    // Si el ejercicio tiene Id, buscarlo en el contexto local
                    Ejercicio ejercicioAdjunto = null;
                    if (ejercicio.Id > 0)
                    {
                        ejercicioAdjunto = _context.Ejercicios.Local
                            .FirstOrDefault(e => e.Id == ejercicio.Id)
                            ?? await _context.Ejercicios.FindAsync(ejercicio.Id);
                    }

                    if (ejercicioAdjunto != null)
                    {
                        // Ya existe y está rastreado, lo agregamos a la rutina
                        rutinaExistente.Ejercicios.Add(ejercicioAdjunto);
                    }
                    else
                    {
                        // Es nuevo, lo agregamos directamente
                        rutinaExistente.Ejercicios.Add(ejercicio);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return rutinaExistente;
        }

        public async Task<bool> EliminarRutinaAsync(int id)
        {
            var rutina = await _context.Rutinas.FindAsync(id);
            if (rutina == null)
            {
                return false;
            }

            _context.Rutinas.Remove(rutina);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
