using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IConfiguracionGrupoMuscularRepositorio
    {
        Task<List<ConfiguracionGrupoMuscular>> ObtenerTodosAsync();
        Task<ConfiguracionGrupoMuscular?> ObtenerPorIdAsync(long id);
        Task<ConfiguracionGrupoMuscular?> AgregarAsync(ConfiguracionGrupoMuscular configuracionGrupoMuscular);
        Task<ConfiguracionGrupoMuscular?> ActualizarAsync(ConfiguracionGrupoMuscular configuracionGrupoMuscular);
        Task EliminarAsync(long id);
    }
}
