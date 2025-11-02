using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.CalculoPuntajeCasosDeUso
{
    public class ObtenerRankingSociosCasoDeUso
    {
        private readonly ISocioRepositorio _socioRepositorio;
    

        public ObtenerRankingSociosCasoDeUso(ISocioRepositorio socioRepositorio)
        {
            _socioRepositorio = socioRepositorio;
        }

        public async Task<List<(long SocioId, string NombreCompleto, double PuntajeTotal)>> Ejecutar()
        {
            var socios = await _socioRepositorio.ObtenerTodosConEntrenamientoAsync();
            var ranking = socios.Select(s =>
            {
                var actividades = s.Entrenamientos?
                    .SelectMany(e => e.Actividades ?? Enumerable.Empty<Domain.Entities.Actividad>())
                    ?? Enumerable.Empty<Domain.Entities.Actividad>();

                double puntajeTotal = actividades.Sum(a => a.Punto ?? 0);

                return (SocioId: s.Id, NombreCompleto: $"{s.Nombre} {s.Apellido}", PuntajeTotal: puntajeTotal);
            })
            .OrderByDescending(s => s.PuntajeTotal)
            .ToList();
            return ranking;
        }
    }
}
