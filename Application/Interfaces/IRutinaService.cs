using FitRank_API.Application.DTOs.RutinaNamespace;
using FitRank_API.Application.DTOs.RutinaNameSpace;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Interfaces
{
    public interface IRutinaService
    {
        //RUTINAS
        Task<CrearRutinaDTO> CrearRutinaAsync(CrearRutinaDTO nuevaRutina);
        Task<List<RutinaDTO>> ListarRutinasAsync();
        Task<List<RutinaDTO>> ListarRutinasPorUsuarioAsync(int usuarioId);
        Task<CrearRutinaDTO> ObtenerRutinaPorIdAsync(int id);
        Task<EditarRutinaDTO> EditarRutinaAsync(int id, EditarRutinaDTO rutinaActualizada);
        Task<bool> EliminarRutinaAsync(int id);

        //BLOQUES
        //Task<Bloque> AgregarBloqueAsync(int rutinaId, BloqueDTO nuevoBloque);
        //Task<Bloque> ObtenerBloquePorIdAsync(int id);
        //Task<Bloque> EditarBloqueAsync(int id, BloqueDTO bloqueActualizado);
        //Task<bool> EliminarBloqueAsync(int id);
    }
}
