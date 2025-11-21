using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
        public interface IEntrenamientoRepositorio
        {
                Task<IEnumerable<Entrenamiento>> ObtenerTodosAsync();
                Task<Entrenamiento?> ObtenerPorIdAsync(long id);
                Task<Entrenamiento> AgregarAsync(Entrenamiento entrenamiento);
                Task<Entrenamiento?> ActualizarAsync(Entrenamiento entrenamiento);
                Task<bool> EliminarAsync(long id);
                Task<IEnumerable<Entrenamiento>> ObtenerPorSocioAsync(long socioId);
                Task<Socio?> ObtenerSocioPorIdAsync(long socioId);
                Task<Entrenamiento?> ObtenerEntrenamientoActivoPorSocioIdAsync(long socioId);
                Task<List<Entrenamiento>> ObtenerHistorialCompletoPorSocioAsync(long socioId);
                Task<List<Entrenamiento>> ObtenerHistorialPorProfesorAsync(long profesorId, string? nombre);

    }
}
