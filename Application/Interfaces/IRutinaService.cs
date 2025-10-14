using FitRank_API.Application.DTOs.RutinaNamespace;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Interfaces
{
    public interface IRutinaService
    {
        //RUTINAS
        Task<IEnumerable<Rutina>> ListarRutinasAsync();
        Task<IEnumerable<Rutina>> ListarRutinasPorUsuarioAsync(int usuarioId);
        Task<Rutina> ObtenerRutinaPorIdAsync(int id);
        Task<Rutina> EditarRutinaAsync(int id, RutinaDTO rutinaActualizada);
        Task<bool> EliminarRutinaAsync(int id);

        //BLOQUES
        Task<Bloque> AgregarBloqueAsync(int rutinaId, BloqueDTO nuevoBloque);
        Task<Bloque> ObtenerBloquePorIdAsync(int id);
        Task<Bloque> EditarBloqueAsync(int id, BloqueDTO bloqueActualizado);
        Task<bool> EliminarBloqueAsync(int id);
    }
}
