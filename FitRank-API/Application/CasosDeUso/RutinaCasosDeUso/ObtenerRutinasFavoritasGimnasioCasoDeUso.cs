using FitRank_API.Infrastructure.Interfaces;

public class ObtenerRutinasFavoritasGimnasioCasoDeUso
{
    private readonly IRutinaRepositorio _repo;
    private readonly ISocioRepositorio _repoSocio;

    public ObtenerRutinasFavoritasGimnasioCasoDeUso(
        IRutinaRepositorio repo,
        ISocioRepositorio repoSocio)
    {
        _repo = repo;
        _repoSocio = repoSocio;
    }

    public async Task<List<RutinaFavoritaGimDTO>> Ejecutar(long gimnasioId)
    {
        // 1️⃣ Obtener todos los socios del gimnasio
        var socios = await _repoSocio.ObtenerTodosPorGimnasio(gimnasioId);

        var socioIds = socios.Select(s => s.Id).ToList();

        
        var rutinas = await _repo.ObtenerRutinasFavoritasPorSociosAsync(socioIds);

        // 3️⃣ Hacer ranking global
        var ranking = rutinas
            .GroupBy(r => r.Nombre) // agrupamos por nombre
            .Select(g => new RutinaFavoritaGimDTO
            {
                Nombre = g.Key,
                Descripcion = g.First().Descripcion,
                CantidadFavoritos = g.Count(),
                UltimaFecha = g.Max(r => r.FechaCreacion)
            })
            .OrderByDescending(x => x.CantidadFavoritos)
            .ToList();

        return ranking;
    }
}
