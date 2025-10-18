using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;

public interface IGrupoMuscularRepositorio
{
    Task<List<GrupoMuscular>> ObtenerTodosAsync();
    Task<GrupoMuscular?> ObtenerPorIdAsync(long id);
    Task<GrupoMuscular?> AgregarAsync(GrupoMuscular grupoMuscular);
    Task<GrupoMuscular?> ActualizarAsync(GrupoMuscular grupoMuscular);
    Task EliminarAsync(long id);



}
