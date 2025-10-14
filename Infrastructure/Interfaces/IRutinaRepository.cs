using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IRutinaRepository
    {
        //RUTINAS
        Task<Rutina> CrearRutinaAsync(Rutina rutina);
        Task<List<Rutina>> ListarRutinasAsync();
        Task<List<Rutina>> ListarRutinasPorUsuarioAsync(int usuarioId);
        Task<Rutina> ObtenerRutinaPorIdAsync(int id);
        Task<Rutina> ActualizarRutinaAsync(Rutina rutina);
        Task<bool> EliminarRutinaAsync(int id);
    }
}
