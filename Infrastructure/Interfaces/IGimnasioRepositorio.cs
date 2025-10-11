using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;
public interface IGimnasioRepositorio
{
    Task<List<Logro>> ListarLogrosActivosAsync(int idGimnasio);
    Task SetEstadoLogro(int idGimnasio, int logroId, bool activo);
    Task OtorgarLogroAsync(int socioId, int logroId, int gimnasioId);
}
