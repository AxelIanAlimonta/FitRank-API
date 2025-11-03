using FitRank_API.Application.DTOs.ActividadDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Strategy;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FitRank_API.Application.UseCases
{
    public class RegistrarActividadCasoDeUso
    {
        private readonly IActividadRepositorio _actividadRepo;
        private readonly IEntrenamientoRepositorio _entrenamientoRepo;
        private readonly IEjercicioAsignadoRepositorio _ejercicioAsignadoRepo;
        private readonly ISesionRepositorio _sesionRepo;
        private readonly FitRankDbContext _context; // si usás EF directamente
        private readonly IRutinaRepositorio _rutinaRepo;

        public RegistrarActividadCasoDeUso(
            IActividadRepositorio actividadRepo,
            IEntrenamientoRepositorio entrenamientoRepo,
            FitRankDbContext context,
            IEjercicioAsignadoRepositorio ejercicioAsignadoRepo,
            ISesionRepositorio sesionRepositorio,
            IRutinaRepositorio rutinaRepositorio)
        {
            _actividadRepo = actividadRepo;
            _entrenamientoRepo = entrenamientoRepo;
            _context = context;
            _ejercicioAsignadoRepo = ejercicioAsignadoRepo;
            _sesionRepo = sesionRepositorio;
            _rutinaRepo = rutinaRepositorio;
        }

        public virtual async Task<Domain.Entities.Actividad> Ejecutar(RegistrarActividadDTO dto)
        {
            // 1️⃣ Obtener la serie y socio
            var serie = await _actividadRepo.ObtenerSeriePorIdAsync(dto.SerieId);
            var ejercicioAsignado = await _ejercicioAsignadoRepo.ObtenerPorIdAsync(serie.EjercicioAsignadoId);
            var sesion = await _sesionRepo.ObtenerPorIdAsync(ejercicioAsignado.SesionId);
            var rutina = await _rutinaRepo.ObtenerPorIdAsync(sesion.RutinaId);
            var socio = await _entrenamientoRepo.ObtenerSocioPorIdAsync(rutina.SocioId);
            var ultimaMedida = socio.MedidasCorporales.OrderByDescending(m => m.Fecha).First();

            // 2️⃣ Configuración del grupo muscular
            var configGrupo = await _context.ConfiguracionesGrupoMuscular
                .FirstOrDefaultAsync(c => c.GrupoMuscularId == serie.EjercicioAsignado.Ejercicio.GrupoMuscularId);

            double multiplicadorPeso = configGrupo?.MultiplicadorPeso ?? 0.1;
            double multiplicadorReps = configGrupo?.MultiplicadorRepeticiones ?? 0.1;

            var ejercicio = await _context.Ejercicios
                .FirstOrDefaultAsync(e => e.Id == serie.EjercicioAsignado.EjercicioId);

            if (ejercicio == null || socio == null)
                throw new Exception("No se pudo obtener socio o ejercicio.");

            // 3️⃣ Calcular puntos
            var calculo = new CalculoGenerico();
            var resultado = calculo.CalcularPuntos(
                ejercicio,
                dto.Repeticiones,
                dto.Repeticiones,
                dto.Peso,
                socio,
                ultimaMedida,
                multiplicadorPeso,
                multiplicadorReps
            );

            // 4️⃣ Verificar entrenamiento activo
            var entrenamiento = await _entrenamientoRepo.ObtenerEntrenamientoActivoPorSocioIdAsync(socio.Id);

            if (entrenamiento == null)
            {
                entrenamiento = new FitRank_API.Domain.Entities.Entrenamiento
                {
                    SocioId = socio.Id,
                    Fecha = DateTime.Now,
                    Duracion = dto.Duracion // si lo tenés
                };
                await _entrenamientoRepo.AgregarAsync(entrenamiento);
            }

            // 5️⃣ Registrar actividad
            var actividad = new Domain.Entities.Actividad
            {
                EntrenamientoId = entrenamiento.Id,
                SerieId = dto.SerieId,
                EjercicioAsignadoId = ejercicio.Id,
                Peso = dto.Peso,
                Repeticiones = dto.Repeticiones,
                Duracion = dto.Duracion, // si lo tenés
                Punto = resultado.Puntos
            };

            await _actividadRepo.AgregarAsync(actividad);

            return actividad;
        }
    }
}
