using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces;

public interface ILogroRepositorio
{
    Task<List<Logro>> ObtenerTodosLosLogros();
    Task<Logro?> ObtenerLogroPorId(long id);
    Task<Logro> AgregarLogro(Logro logro);
    Task<Logro?> ActualizarLogro(long id, Application.DTOs.LogroDTOs.ActualizarLogroDTO logroDTO);
    Task<bool> EliminarLogro(long id);
}
