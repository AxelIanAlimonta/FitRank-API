using FitRank_API.Application.DTOs.PuntajeDTOs;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.CalculoPuntajeCasosDeUso
{
    public class ObtenerRankingSociosCasoDeUso
    {
        private readonly ISocioRepositorio _socioRepositorio;


        public ObtenerRankingSociosCasoDeUso(ISocioRepositorio socioRepositorio)
        {
            _socioRepositorio = socioRepositorio;
        }

        //public async Task<List<SocioRankingDto>> Ejecutar(long gimnasioId, int cantidad)
        //{
        //    var socios = await _socioRepositorio.ObtenerSociosParaRankingAsync(gimnasioId);
        //    foreach (var s in socios)
        //    {
        //        Console.WriteLine($"{s.Id} - {s.Nombre} {s.Apellido}");
        //    }
        //    var ranking = socios.Select(s =>
        //    {
        //        var actividades = s.Entrenamientos?
        //            .SelectMany(e => e.Actividades ?? Enumerable.Empty<Domain.Entities.Actividad>())
        //            ?? Enumerable.Empty<Domain.Entities.Actividad>();

        //        double puntajeTotal = actividades.Sum(a => a.Punto ?? 0);

        //        return new SocioRankingDto
        //        {
        //            SocioId = s.Id,
        //            NombreCompleto = $"{s.Nombre} {s.Apellido}",
        //            PuntajeTotal = puntajeTotal
        //        };
        //    })
        //    .OrderByDescending(s => s.PuntajeTotal)
        //    .Take(cantidad)
        //    .ToList();
        //    return ranking;
        //}

        public virtual async Task<List<SocioRankingDto>> Ejecutar(long gimnasioId, int cantidad)
        {
            return await _socioRepositorio.ObtenerRankingGeneralAsync(gimnasioId, cantidad);
        }
    }
}