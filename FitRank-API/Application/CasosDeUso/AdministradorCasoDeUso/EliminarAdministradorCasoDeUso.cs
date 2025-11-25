using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.CasosDeUso.AdministradorCasosDeUso
{
    public class EliminarAdministradorCasoDeUso
    {
        private readonly IAdministradorRepositorio _adminRepositorio;

        public EliminarAdministradorCasoDeUso(IAdministradorRepositorio adminRepositorio)
        {
            _adminRepositorio = adminRepositorio;
        }

        public virtual async Task<bool> Ejecutar(long id)
        {
            var admin = await _adminRepositorio.ObtenerPorIdAsync(id);

            if (admin == null)
                return false;

            await _adminRepositorio.EliminarAsync(admin);
            return true;
        }
    }
}

