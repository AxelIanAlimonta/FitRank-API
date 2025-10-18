using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;

public interface IGrupoMuscularRepositorio
{
    Task<List<GrupoMuscular>> ObtenerTodosAsync();
    Task<GrupoMuscular?> ObtenerPorIdAsync(long id);
    Task AgregarAsync(GrupoMuscular grupoMuscular);
    Task ActualizarAsync(GrupoMuscular grupoMuscular);
    Task EliminarAsync(long id);



}
