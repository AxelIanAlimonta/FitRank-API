using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IMedidaCorporalRepositorio
    {
        Task<MedidaCorporal> AgregarAsync(MedidaCorporal medida);
        Task<MedidaCorporal?> ObtenerPorIdAsync(long id);
        Task<List<MedidaCorporal>> ObtenerPorSocioAsync(long socioId);
        Task<MedidaCorporal?> ActualizarAsync(MedidaCorporal medida);
        Task<bool> EliminarAsync(long id);

        Task<MedidaCorporal?> ObtenerUltimaMedidaPorSocioAsync(long socioId);
    }
}
