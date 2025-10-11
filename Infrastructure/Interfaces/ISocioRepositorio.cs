using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface ISocioRepositorio
    {
        Task<List<SocioRealizaLogro>> MisLogrosAsync(int socioId);
    }
}
