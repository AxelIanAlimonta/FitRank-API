using FitRank_API.Application.DTOs.Rutina;

namespace FitRank_API.Application.Interfaces
{
    public interface IRutinaServicio
    {
        Task<RutinaDTO> CrearRutinaAsync(CrearRutinaDTO dto);
        Task<RutinaDTO> ObtenerRutinaAsync(int id);
        Task<List<RutinaDTO>> ListarRutinasAsync();
        Task<RutinaDTO> ActualizarRutinaAsync(int id, ActualizarRutinaDTO dto);
        Task<bool> EliminarRutinaAsync(int id);
    }
}
