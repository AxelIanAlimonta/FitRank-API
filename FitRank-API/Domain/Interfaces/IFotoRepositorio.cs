using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Interfaces
{
    public interface IFotoRepositorio
    {
        Task<Foto> AgregarAsync(Foto foto);
        Task<IEnumerable<Foto>> ObtenerPorSocioAsync(long socioId);
        Task<bool> EliminarAsync(long id);
    }
}
