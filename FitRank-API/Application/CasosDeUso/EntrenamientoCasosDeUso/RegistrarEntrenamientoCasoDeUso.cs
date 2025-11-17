using FitRank_API.Application.DTOs.ActividadDTOs;
using FitRank_API.Application.DTOs.EntrenamientoDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Strategy;
using FitRank_API.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Application.CasosDeUso.EntrenamientoCasosDeUso
{
    public class RegistrarEntrenamientoCasoDeUso
    {
        private readonly IEntrenamientoRepositorio _entrenamientoRepo;
        private readonly IActividadRepositorio _actividadRepo;
        private readonly FitRank_API.Infrastructure.Persistence.FitRankDbContext _context;

        public RegistrarEntrenamientoCasoDeUso(
            IEntrenamientoRepositorio entrenamientoRepo,
            IActividadRepositorio actividadRepo,
            Infrastructure.Persistence.FitRankDbContext context)
        {
            _entrenamientoRepo = entrenamientoRepo;
            _actividadRepo = actividadRepo;
            _context = context;
        }

        public async Task<ObtenerEntrenamientoConPuntaje> Ejecutar(RegistrarEntrenamientoConActividadesDTO dto)
        {
            // 1️⃣ Crear entrenamiento
            var entrenamiento = new Entrenamiento
            {
                SocioId = dto.SocioId,
                Fecha = dto.Fecha,
                Duracion = dto.Duracion
            };
            entrenamiento = await _entrenamientoRepo.AgregarAsync(entrenamiento);

            double puntajeTotal = 0;
            var resultados = new List<ObtenerActividadConPuntajeDTO>();

            // 2️⃣ Recorrer actividades y calcular puntaje
            foreach (var actividadDto in dto.Actividades)
            {
                // Obtener serie y socio
                var serie = await _actividadRepo.ObtenerSeriePorIdAsync(actividadDto.SerieId);
                var socio = await _entrenamientoRepo.ObtenerSocioPorIdAsync(dto.SocioId);
                var ultimaMedida = socio.MedidasCorporales.OrderByDescending(m => m.Fecha).First();

                var configGrupo = await _context.ConfiguracionesGrupoMuscular
                    .FirstOrDefaultAsync(c => c.GrupoMuscularId == serie.EjercicioAsignado.Ejercicio.GrupoMuscularId);

                double multiplicadorPeso = configGrupo?.MultiplicadorPeso ?? 0.1;
                double multiplicadorReps = configGrupo?.MultiplicadorRepeticiones ?? 0.1;

                var ejercicio = await _context.Ejercicios
                    .Include(e => e.GrupoMuscular)
                    .ThenInclude(gm => gm.Configuraciones)
                    .FirstOrDefaultAsync(e => e.Id == serie.EjercicioAsignado.EjercicioId);
                if (ejercicio == null || socio == null)
                    continue;

                // Calcular puntos
                var calculo = new CalculoGenerico();
                var resultado = calculo.CalcularPuntos(
                    ejercicio,
                    actividadDto.Repeticiones ?? 0,
                    actividadDto.Repeticiones ?? 0,
                    actividadDto.Peso ?? 0,
                    socio,
                    ultimaMedida,
                    multiplicadorPeso,
                    multiplicadorReps
                );

                // Crear actividad
                var actividad = new Actividad
                {
                    SerieId = actividadDto.SerieId,
                    EntrenamientoId = entrenamiento.Id,
                    EjercicioAsignadoId = actividadDto.EjercicioAsignadoId,
                    Repeticiones = actividadDto.Repeticiones,
                    Peso = actividadDto.Peso,
                    Duracion = actividadDto.Duracion,
                    Punto = resultado.Puntos
                };

                await _actividadRepo.AgregarAsync(actividad);

                puntajeTotal += resultado.Puntos;

                resultados.Add(new ObtenerActividadConPuntajeDTO
                {
                    SerieId = actividadDto.SerieId,
                    Puntos = resultado.Puntos,
                    MensajeAdvertencia = resultado.MensajeAdvertencia
                });
            }

            // 3️⃣ Devolver resumen
            return new ObtenerEntrenamientoConPuntaje
            {
                EntrenamientoId = entrenamiento.Id,
                PuntosTotales = Math.Round(puntajeTotal, 2),
                Actividades = resultados
            };
        }
    }
}
