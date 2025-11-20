using FitRank_API.Domain.Entities;

namespace FitRank_API.Infrastructure.Interfaces
{
    public interface IInvitacionRepositorio
    {
        Task<Invitacion?> ObtenerPorIdAsync(long id);
      
        Task<Invitacion> AgregarAsync(Invitacion invitacion);
        Task<Invitacion> ActualizarAsync(Invitacion invitacion);
        Task<bool> Eliminar(long id);
        Task<Invitacion> ObtenerPorIdYEstadoAsync(long invitacionId, string v);

        Task<List<Invitacion>> ObtenerTodasAsync(int gimnasioId);

        Task<Invitacion?> ObtenerPorEmailAsync(string email);

    }
}
