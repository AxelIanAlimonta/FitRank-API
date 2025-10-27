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

        public async Task<bool> Ejecutar(long socioId, string rol, long id)
        {
            var medida = await _repo.ObtenerPorIdAsync(id);
            if (medida == null)
                return false;

            
            if (rol == "Socio" && medida.SocioId != socioId)
                throw new UnauthorizedAccessException("No estás autorizado para eliminar esta medición.");

            await _repo.EliminarAsync(medida);
            return true;
        }
    }
}
