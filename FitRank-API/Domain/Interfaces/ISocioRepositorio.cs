using FitRank_API.Application.DTOs.PuntajeDTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Domain.Interfaces;

public interface ISocioRepositorio
{
    Task<List<Socio>> ObtenerTodosAsync();
    Task<Socio?> ObtenerPorIdAsync(long id);
    Task<Socio> AgregarAsync(Socio socio);
    Task<Socio?> ActualizarAsync(Socio socio);
    Task<bool> EliminarAsync(long id);
    Task<Socio?> ObtenerSocioConMedidasAsync(long socioId);
    Task<Socio?> ObtenerSocioConEntrenamientosAsync(long socioId);
    Task<IEnumerable<Socio>> ObtenerTodosConEntrenamientoAsync();

    Task<IEnumerable<Socio>> ObtenerTodosPorGimnasio(long gimnasioId);
    Task<bool> CambiarParticipacionRankingAsync(long socioId, bool participa);
    Task<IEnumerable<Socio>> ObtenerSociosParaRankingAsync(long gimnasioId);
    Task<List<SocioRankingDto>> ObtenerRankingGeneralAsync(long gimnasioId, int cantidad);

    Task<Socio?> ObtenerSocioYUsuarioPorIdAsync(long socioId);
}
