using AutoMapper;
using FitRank_API.Application.DTOs.EntrenamientoDTOs;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.EntrenamientoCasosDeUso
{
    public class ObtenerHistorialEntrenamientosDeUnUsuarioCasoDeUso
    {
        private readonly IEntrenamientoRepositorio _entrenamientoRepositorio;
        private readonly IMapper _mapper;

        public ObtenerHistorialEntrenamientosDeUnUsuarioCasoDeUso(IEntrenamientoRepositorio entrenamientoRepositorio,
        IMapper mapper)
        {
            _entrenamientoRepositorio = entrenamientoRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<List<EntrenamientoHistorialDTO>> EjecutarAsync(long socioId)
        {
            var entrenamientos = await _entrenamientoRepositorio.ObtenerHistorialCompletoPorSocioAsync(socioId);

            var historial = _mapper.Map<List<EntrenamientoHistorialDTO>>(entrenamientos);

            var hace30 = DateTime.UtcNow.AddDays(-30);

            foreach (var entrenamientoDTO in historial)
            {
                foreach (var actividadDTO in entrenamientoDTO.Actividades)
                {
                    var actividadesReales = entrenamientos
                        .SelectMany(e => e.Actividades)
                        .Where(a => a.EjercicioAsignadoId == actividadDTO.IdEjercicioAsignado)
                        .OrderBy(a => a.Entrenamiento.Fecha)
                        .ToList();

                    actividadDTO.ProgresoHistorico =
                        _mapper.Map<List<ProgresoEjercicioDTO>>(actividadesReales)
                        .Where(p => p.Fecha >= hace30)
                        .ToList();

                }
            }

            return historial;
        }
    }
}
