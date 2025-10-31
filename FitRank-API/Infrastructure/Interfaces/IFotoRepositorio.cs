using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IFotoRepositorio
    {
        Task<Foto> AgregarAsync(Foto foto);
        Task<IEnumerable<Foto>> ObtenerPorSocioAsync(long socioId);
        Task<bool> EliminarAsync(long id);
    }
}
