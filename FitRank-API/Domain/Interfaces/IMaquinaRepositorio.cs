using FitRank_API.Application.DTOs.MaquinaDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Interfaces
{
    public interface IMaquinaRepositorio
    {
        Task<List<Maquina>> ObtenerTodasLasMaquinas();
        Task<Maquina?> ObtenerMaquinaPorId(long id);
        Task<Maquina> AgregarMaquina(Maquina m);
        Task<Maquina?> ActualizarMaquina(Maquina m);
        Task<bool> EliminarMaquina(long id);


    }
}
