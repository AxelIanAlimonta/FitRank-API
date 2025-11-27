using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Interfaces
{
        public interface IActividadRepositorio
        {
                Task<IEnumerable<Actividad>> ObtenerTodasAsync();
                Task<Actividad?> ObtenerPorIdAsync(long id);
                Task<IEnumerable<Actividad>> ObtenerPorSerieAsync(long serieId);
                Task<Actividad> AgregarAsync(Actividad actividad);
                Task<IEnumerable<Actividad>> ObtenerPorEntrenamientoAsync(long id);
                Task<Serie> ObtenerSeriePorIdAsync(long serieId);
                Task<Actividad?> ActualizarAsync(Actividad actividad);
                Task<bool> EliminarAsync(long id);
        }
}
