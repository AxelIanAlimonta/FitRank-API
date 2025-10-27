using FitRank_API.Application.DTOs.MaquinaDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IMaquinaRepositorio
    {
        Task<List<Maquina>> ObtenerTodasLasMaquinas();
        Task<Maquina?> ObtenerMaquinaPorId(long id);
        Task<Maquina> AgregarMaquina(Maquina m);
        Task<Maquina?> ActualizarMaquina(long id, ActualizarMaquinaDTO dto);
        Task<bool> EliminarMaquina(long id);
    }
}
