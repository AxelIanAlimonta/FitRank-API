using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.Invitacion
{
    public class EliminarInvitacionCasoDeUso
    {
        private readonly IInvitacionRepositorio _invitacionRepositorio;

        public EliminarInvitacionCasoDeUso(IInvitacionRepositorio invitacionRepositorio)
        {
            _invitacionRepositorio = invitacionRepositorio;
        }

        public async Task<bool> Ejecutar(long id)
        {
            var eliminado = await _invitacionRepositorio.Eliminar(id);
            if (!eliminado)
                throw new Exception("No se encontró la invitación para eliminar.");

            return true;
        }
    }
}
