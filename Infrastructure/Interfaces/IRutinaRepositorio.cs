using FitRank_API.Application.DTOs.Rutina;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IRutinaRepositorio
    {
        Task<Rutina> CrearRutinaAsync(Rutina rutina);
        Task<Rutina?> ObtenerRutinaAsync(int id);
        Task<List<Rutina>> ListarRutinasAsync();
        Task<Rutina> ActualizarAsync(Rutina rutina);
        Task EliminarRutinaAsync(Rutina rutina);
    }
}
