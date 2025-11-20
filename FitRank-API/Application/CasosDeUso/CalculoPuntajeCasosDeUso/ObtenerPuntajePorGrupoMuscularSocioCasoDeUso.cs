using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.CalculoPuntajeCasosDeUso
{
    public class ObtenerPuntajePorGrupoMuscularSocioCasoDeUso
    {
        private readonly ISocioRepositorio _socioRepositorio;
        public ObtenerPuntajePorGrupoMuscularSocioCasoDeUso(ISocioRepositorio socioRepositorio)
        {
            _socioRepositorio = socioRepositorio;
        }

        public async Task<Dictionary<string, double>> Ejecutar(long socioId)
        {
            var socio = await _socioRepositorio.ObtenerSocioConEntrenamientosAsync(socioId);
            if (socio == null || socio.Entrenamientos == null)
            {
                return new Dictionary<string, double>();
            }

            var actividades = socio.Entrenamientos
                .SelectMany(e => e.Actividades ?? Enumerable.Empty<Domain.Entities.Actividad>())
                .ToList();
            
            return actividades
                .GroupBy(a=> a.Serie?.EjercicioAsignado?.Ejercicio?.GrupoMuscular?.Nombre??"Desconocido")
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(a => a.Punto??0)
                );
        }
    }
}
