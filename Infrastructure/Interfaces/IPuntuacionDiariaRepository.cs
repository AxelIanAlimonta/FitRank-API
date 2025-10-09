using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IPuntuacionDiariaRepository
    {
        Task<PuntuacionDiaria> GetByUsuarioYFechaAsync(int usuarioId, DateTime fechaHoy);
        Task ModificarPuntuacionDiaria(PuntuacionDiaria puntuacionDiaria);
        Task RegistrarPuntuacionDiaria(PuntuacionDiaria puntuacionDiaria);
    }
}
