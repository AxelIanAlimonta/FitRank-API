using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.CalculoPuntajeCasosDeUso
{
    public class ObtenerPuntajeTotalSocioCasoDeUso
    {
        private readonly ISocioRepositorio _socioRepositorio;

        public ObtenerPuntajeTotalSocioCasoDeUso(ISocioRepositorio socioRepositorio)
        {
            _socioRepositorio = socioRepositorio;
        }

        public virtual async Task<double> Ejecutar(long socioId)
        {
            var socio = await _socioRepositorio.ObtenerSocioConEntrenamientosAsync(socioId);
            if (socio == null || socio.Entrenamientos == null) return 0;

            var actividades = socio.Entrenamientos
                .SelectMany(e => e.Actividades ?? Enumerable.Empty<Domain.Entities.Actividad>())
                .ToList();

            return actividades.Sum(a => a.Punto ?? 0);
        }
    }
}
