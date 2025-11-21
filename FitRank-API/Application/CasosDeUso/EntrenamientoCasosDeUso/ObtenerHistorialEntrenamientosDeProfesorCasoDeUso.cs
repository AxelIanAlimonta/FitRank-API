using AutoMapper;
using FitRank_API.Application.DTOs.EntrenamientoDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.EntrenamientoCasosDeUso
{
    public class ObtenerHistorialEntrenamientosDeProfesorCasoDeUso
    {
        private readonly IEntrenamientoRepositorio _entrenamientoRepositorio;
        private readonly IMapper _mapper;

        public ObtenerHistorialEntrenamientosDeProfesorCasoDeUso(IEntrenamientoRepositorio repo, IMapper mapper)
        {
            _entrenamientoRepositorio = repo;
            _mapper = mapper;
        }

        public async Task<List<EntrenamientoHistorialDTO>> EjecutarAsync(long profesorId, string? nombre)
        {
            var entrenamientos = await _entrenamientoRepositorio.ObtenerHistorialPorProfesorAsync(profesorId, nombre);

            var historial = _mapper.Map<List<EntrenamientoHistorialDTO>>(entrenamientos);

            var hace30 = DateTime.UtcNow.AddDays(-30);

            // 🚀 Precomputamos todas las actividades por ejercicio
            var todasLasActividades = entrenamientos
                .SelectMany(e => e.Actividades)
                .GroupBy(a => a.EjercicioAsignadoId)
                .ToDictionary(g => g.Key, g => g.OrderBy(a => a.Entrenamiento.Fecha).ToList());

            // 🔁 Para cada actividad del DTO, seteamos el progreso histórico
            foreach (var entrenamientoDTO in historial)
            {
                foreach (var actividadDTO in entrenamientoDTO.Actividades)
                {
                    if (todasLasActividades.TryGetValue(actividadDTO.IdEjercicioAsignado, out var actividadesReales))
                    {
                        actividadDTO.ProgresoHistorico = _mapper.Map<List<ProgresoEjercicioDTO>>(actividadesReales)
                            .Where(p => p.Fecha >= hace30)
                            .ToList();
                    }
                }
            }

            return historial;
        }

    }
}
