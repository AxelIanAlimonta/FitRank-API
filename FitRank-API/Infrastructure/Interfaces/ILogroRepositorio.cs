using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;

public interface ILogroRepositorio
{
    Task<List<Logro>> ObtenerTodosLosLogros();
    Task<Logro?> ObtenerLogroPorId(long id);
    Task<Logro> AgregarLogro(Logro logro);
    Task<Logro?> ActualizarLogro(Logro logro);
    Task<bool> EliminarLogro(long id);
}
