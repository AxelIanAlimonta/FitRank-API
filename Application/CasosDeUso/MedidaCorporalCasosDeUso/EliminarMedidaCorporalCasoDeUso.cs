using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.MedidaCorporalCasosDeUso
{
    public class EliminarMedidaCorporalCasoDeUso
    {
        private readonly IMedidaCorporalRepositorio _repo;

        public EliminarMedidaCorporalCasoDeUso(IMedidaCorporalRepositorio repo)
        {
            _repo = repo;
        }

        public async Task<bool> Ejecutar(long id)
        {
            return await _repo.EliminarAsync(id);
        }
    }
}
