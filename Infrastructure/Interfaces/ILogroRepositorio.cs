using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface ILogroRepositorio
    {
        Task<List<Logro>> ListarActivosAsync(CancellationToken ct = default);
        Task<int> CrearLogroAsync(Logro entity, CancellationToken ct = default);

        Task<List<SocioRealizaLogro>> MisLogrosAsync(int socioId, CancellationToken ct = default);

        Task<SocioRealizaLogro?> OtorgarSiNoExisteAsync(
            int socioId,
            int logroId,
            CancellationToken ct = default);

        Task SetActivoAsync(int logroId, bool activo, CancellationToken ct = default);
    }
}
